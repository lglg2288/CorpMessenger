using Client.Services;
using Grpc.Core;
using Grpc.Net.Client;
using MessengerAvalonia.Shared.CallsGrpc;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Interprice
{
    public class GrpcCallService : ICallService
    {
        private AsyncDuplexStreamingCall<CallMessage, AudioChunk>? _call;
        private CancellationTokenSource _cts = new();
        private readonly AudioService.AudioServiceClient _client;
        private IMicrophoneService _microphoneService;
        public event Action<byte[]>? onAudioChunkReceived;

        public GrpcCallService(GrpcChannel channel, IMicrophoneService microphoneService)
        {
            _client = new AudioService.AudioServiceClient(channel);
            _microphoneService = microphoneService;
        }


        public async Task StartCallAsync(string myLogin, string friendLogin)
        {
            if (_call != null)
                return;
            if (onAudioChunkReceived == null)
                throw new InvalidOperationException("No handler onAudioChunkReceived for received audio chunks.");

            _cts = new CancellationTokenSource();
            _call = _client.AudioCall(cancellationToken: _cts.Token);

            await _call.RequestStream.WriteAsync(new CallMessage
            {
                Join = new JoinInfo
                {
                    MyLogin = myLogin,
                    TargetLogin = friendLogin
                }
            });

            _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var chunk in _call.ResponseStream.ReadAllAsync(_cts.Token))
                    {
                        onAudioChunkReceived.Invoke(chunk.Data.ToByteArray());
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Console.WriteLine("Соединение разорвано: " + ex.Message);
                }
                finally
                {
                    _call = null;
                }
            });
            await _microphoneService.StartRecordingAsync(async chunk =>
            {
                if (_call == null || _cts.IsCancellationRequested) return;

                try
                {
                    await _call.RequestStream.WriteAsync(new CallMessage
                    {
                        Audio = new AudioChunk { Data = Google.Protobuf.ByteString.CopyFrom(chunk.Span) }
                    });
                }
                catch (Exception ex)
                {
                    // ошибка отправки → завершаем звонок
                    await EndCallAsync();
                }
            });
        }
        public async Task EndCallAsync()
        {
            _cts?.Cancel();
            if (_call != null)
            {
                await _call.RequestStream.CompleteAsync();
                _call.Dispose();
                _call = null;
            }
            await _microphoneService.StopRecordingAsync();
            // обнови UI, статус "Звонок завершён"
        }
    }
}