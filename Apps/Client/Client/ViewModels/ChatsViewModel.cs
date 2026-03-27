using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessengerAvalonia.Shared.FriendsGrpc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Client.ViewModels
{
    public partial class ChatsViewModel : ViewModelBase
    {
        private readonly MainViewModel _parent;
        private readonly IFriendService _friendService;
        private Timer _updateTimer;
        private readonly IServiceProvider _services;

        public ChatsViewModel(IServiceProvider services, MainViewModel parent, IFriendService chatsService)
        {
            _services = services;
            _parent = parent;
            _friendService = chatsService;
            LoadFriendsAsync();
            StartAutoUpdate();
        }

        [ObservableProperty]
        private string? friendsLogin;
        [ObservableProperty]
        private string? statusMessage = "Мои друзья";
        [ObservableProperty]
        private ObservableCollection<string> friends = new();

        [RelayCommand]
        private async Task ClickAddFriendAsync()
        {
            try
            {
                var response = await _friendService.AddFriendAsync(UserSelf.login,
                                                                   UserSelf.password,
                                                                   FriendsLogin);
                StatusMessage = response.Message;                
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
            LoadFriendsAsync();
        }

        private async Task LoadFriendsAsync()
        {
            try
            {
                StatusMessage = "Загрузка...";

                var reply = await _friendService.GetFriendsAsync(UserSelf.login, UserSelf.password);

                Friends.Clear();

                if (reply.Friends.Count == 0)
                {
                    StatusMessage = "У вас нет друзей";
                    return;
                }

                foreach (var friend in reply.Friends)
                {
                    Friends.Add(friend);
                }

                StatusMessage = $"Друзей: {Friends.Count}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
        }

        [RelayCommand]
        private void OpenChat(string friendLogin)
        {
            Models.CurrentChat.friendLogin = friendLogin;
            var vm = ActivatorUtilities.CreateInstance<ChatViewModel>(_services, _parent);
            _parent.CurrentView = null;
            _parent.CurrentView = vm;
        }

        private void StartAutoUpdate()
        {
            _updateTimer = new Timer(3000);
            _updateTimer.Elapsed += async (sender, e) =>
            {
                // Используем Dispatcher.UIThread для обновления UI из другого потока
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    LoadFriendsAsync();
                });
            };
            _updateTimer.AutoReset = true;
            _updateTimer.Start();
        }
    }
}
