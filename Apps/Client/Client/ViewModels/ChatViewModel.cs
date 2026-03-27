using Avalonia.Threading;
using Client.Interprice;
using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using MessengerAvalonia.Shared.FriendsGrpc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Client.ViewModels
{
    public partial class ChatViewModel : ViewModelBase
    {
        private readonly MainViewModel _parent;
        private readonly IChatsService _chatsService;
        private readonly IServiceProvider _services;
        private ICallService _callService;
        private readonly IAudioPlayerService _audioPlayer;
        private Timer _updateTimer;
        [ObservableProperty]
        private ObservableCollection<MessageItem> messages = new();
        [ObservableProperty]
        private string? messageInput;
        [ObservableProperty]
        private string? chatname = CurrentChat.friendLogin;

        public ChatViewModel(IServiceProvider services, MainViewModel parent, IChatsService chatsService, ICallService callService, IAudioPlayerService audioPlayer)
        {
            _services = services;
            _parent = parent;
            _chatsService = chatsService;
            _callService = callService;
            LoadMessagesAsync();
            StartAutoUpdate();
            _audioPlayer = audioPlayer;
        }

        public void AddMessage(string sender, string text)
        {
            var isMe = sender == UserSelf.login;  // или Models.UserSelf.login

            Messages.Add(new MessageItem
            {
                Sender = sender,
                Text = text,
                Timestamp = DateTime.Now,
                IsMyMessage = isMe
            });
        }
        public async Task LoadMessagesAsync()
        {
            var reply = await _chatsService.GetMessages(Models.UserSelf.login,
                                                            Models.UserSelf.password,
                                                            Models.CurrentChat.friendLogin);
            Messages.Clear();

            for (int i = 0; i < reply.Message.Count - 1; i++)
            {
                Messages.Add(new MessageItem
                {
                    Sender = reply.Writer[i],
                    Text = reply.Message[i],
                    Timestamp = DateTime.Now,
                    IsMyMessage = reply.Writer[i] == UserSelf.login
                });
            }
        }
        private void StartAutoUpdate()
        {
            _updateTimer = new Timer(3000);
            _updateTimer.Elapsed += async (sender, e) =>
            {
                // Используем Dispatcher.UIThread для обновления UI из другого потока
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {                    
                    LoadMessagesAsync();
                });
            };
            _updateTimer.AutoReset = true;
            _updateTimer.Start();
        }

        [RelayCommand]
        private async void Call()
        {
            try
            {
                _audioPlayer.Start();
                _callService.onAudioChunkReceived += async (byte[] bytes) =>
                {
                    chatname = bytes.Length.ToString();
                    _audioPlayer.PlayChunk(bytes);
                };
                await _callService.StartCallAsync(Models.UserSelf.login, Models.CurrentChat.friendLogin);

            }
            catch (Exception ex)
            {
                
            }
            return;
        }

        [RelayCommand]
        private async void SendMessageAsync()
        {
            try
            {
                var reply = await _chatsService.SendMessage(Models.UserSelf.login,
                                                            Models.UserSelf.password,
                                                            Models.CurrentChat.friendLogin,
                                                            MessageInput);
                LoadMessagesAsync();
            }
            catch (Exception ex)
            {
                Messages.Add(new MessageItem
                {
                    Sender = "Program Exception",
                    Text = $"Ошибка: {ex.Message}",
                    Timestamp = DateTime.Now,
                    IsMyMessage = true
                });
            }            
        }
        [RelayCommand]
        private void BackToChatsList()
        {
            var vm = ActivatorUtilities.CreateInstance<ChatsViewModel>(_services, _parent);
            _parent.CurrentView = vm;
        }
    }
    public class MessageItem
    {
        public string Sender { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsMyMessage { get; set; }
    }
}
