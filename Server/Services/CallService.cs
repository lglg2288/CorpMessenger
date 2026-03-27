using Grpc.Core;
using MessengerAvalonia.Shared.CallsGrpc;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;

namespace Server.Services
{
    public class CallService : AudioService.AudioServiceBase
    {
        private readonly ILogger<CallService> _logger;
        private readonly CallManager _callManager;

        public CallService(ILogger<CallService> logger, CallManager callManager)
        {
            _callManager = callManager;
            _logger = logger;
        }
        public override async Task AudioCall(IAsyncStreamReader<CallMessage> requestStream,
                                             IServerStreamWriter<AudioChunk> responseStream,
                                             ServerCallContext context)
        {
            string? myLogin = null;
            string? roomKey = null;
            CallParticipant? participant = null;
            try
            {
                // 1. Обрабатываем входящий поток сообщений от клиента
                while (await requestStream.MoveNext(context.CancellationToken))
                {
                    var message = requestStream.Current;

                    switch (message.PayloadCase)
                    {
                        case CallMessage.PayloadOneofCase.Join:
                            // Первое (и единственное) join-сообщение
                            if (myLogin != null)
                            {
                                // Уже присоединялись → ошибка (защита от повторного join)
                                Console.WriteLine("Already joined exception");
                                break;
                            }

                            var join = message.Join;
                            myLogin = join.MyLogin;
                            if (string.IsNullOrWhiteSpace(myLogin) || string.IsNullOrWhiteSpace(join.TargetLogin))
                                throw new RpcException(new Status(StatusCode.InvalidArgument, "Login required"));

                            roomKey = CallManager.Get1to1RoomKey(myLogin, join.TargetLogin);

                            var room = _callManager.GetOrCreateRoom(roomKey);

                            participant = new CallParticipant
                            {
                                Login = myLogin,
                                Stream = responseStream  // сохраняем стрим для отправки аудио этому участнику
                            };

                            lock (room) room.Participants.Add(participant);

                            // Опционально: уведомить других в комнате (через их стримы)
                            // BroadcastText(room, $"{myLogin} joined the call", exclude: myLogin);

                            // Можно отправить подтверждение клиенту, если нужно
                            // await responseStream.WriteAsync(new AudioChunk { Data = ByteString.CopyFromUtf8("joined") });
                            

                            break;
                        case CallMessage.PayloadOneofCase.Audio:
                            
                            if (roomKey == null || participant == null)
                                continue;

                            var audioChunk = message.Audio;

                            // Форвардим всем остальным участникам комнаты (кроме отправителя)
                            var roomToForward = _callManager._rooms.GetValueOrDefault(roomKey);
                            if (roomToForward == null) continue;

                            var toSend = new AudioChunk { Data = audioChunk.Data };  // просто пересылаем

                            foreach (var p in roomToForward.Participants)
                            {
                                if (p.Login == myLogin) continue;          // не шлём себе
                                if (p.Stream == null) continue;

                                try
                                {
                                    await p.Stream.WriteAsync(toSend);
                                }
                                catch
                                {
                                    // клиент отвалился → можно удалить позже в cleanup
                                }
                            }
                            

                            break;

                        case CallMessage.PayloadOneofCase.None:
                            // Пустое сообщение — игнорируем
                            break;

                        default:
                            // Неизвестный case → можно логировать
                            break;
                    }
                }
            }
            catch (RpcException) { throw; }  // пробрасываем известные ошибки
            catch (OperationCanceledException) { }  // нормальное завершение
            catch (Exception ex)
            {
                // Логируем неожиданные ошибки
                _logger.LogError(ex, "Error in AudioCall for {Login}", myLogin);
            }
            finally
            {
                // Cleanup при завершении стрима (клиент отключился или ошибка)
                if (roomKey != null && participant != null)
                {
                    var room = _callManager._rooms.GetValueOrDefault(roomKey);
                    if (room != null)
                    {
                        lock (room) room.Participants.Remove(participant);
                        // Опционально: уведомить остальных "left"
                    }
                }
            }
        }
    }
    public class CallParticipant
    {
        public string Login { get; init; } = null!;
        public IServerStreamWriter<AudioChunk>? Stream { get; set; }   // для отправки аудио другим
        public bool IsMuted { get; set; }
        public DateTime JoinedAt { get; init; } = DateTime.UtcNow;
        public bool IsOnline = false;
    }

    public class CallRoom
    {
        public string RoomKey { get; init; } = null!;                  // "1to1:alice:bob"
        public List<CallParticipant> Participants { get; } = new();
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }


    public class CallManager
    {
        public readonly ConcurrentDictionary<string, CallRoom> _rooms = new();

        public CallRoom GetOrCreateRoom(string key)
        {
            return _rooms.GetOrAdd(key, _ => new CallRoom
            {
                RoomKey = key,           // удобно сохранить ключ и внутри комнаты
                CreatedAt = DateTime.UtcNow
            });
        }

        public void Cleanup()
        {
            foreach (var kv in _rooms)
            {
                var room = kv.Value;
                room.Participants.RemoveAll(p => !p.IsOnline);

                if (room.Participants.Count == 0)
                    _rooms.TryRemove(kv.Key, out _);
            }
        }

        // Вызывать раз в 30–60 сек из BackgroundService
        public void StartCleanupTimer()
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    Cleanup();
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
            });
        }
        public static string Get1to1RoomKey(string userA, string userB)
        {
            var comparison = string.Compare(userA, userB, StringComparison.Ordinal);
            return comparison <= 0
                ? $"1to1:{userA}:{userB}"
                : $"1to1:{userB}:{userA}";
        }
        // ... методы Join, Leave, Broadcast и т.д.
    }
}