using Avalonia.Threading;
using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Protobuf;
using MessengerAvalonia.Shared.FriendsGrpc;
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
        private Timer _updateTimer;
        [ObservableProperty]
        private ObservableCollection<MessageItem> messages = new();
        [ObservableProperty]
        private string? messageInput;

        public ChatViewModel(MainViewModel parent, IChatsService chatsService)
        {
            _parent = parent;
            _chatsService = chatsService;
            LoadMessagesAsync();
            StartAutoUpdate();
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
            _parent.CurrentView = new ChatsViewModel(_parent,
                        new Interprice.GrpcFriendService(Models.CurrentGrpcChannel.channel)); ;
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
