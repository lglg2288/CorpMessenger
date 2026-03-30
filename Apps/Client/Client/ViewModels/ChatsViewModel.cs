using Avalonia.Threading;
using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessengerAvalonia.Shared.FriendsGrpc;
using MessengerAvalonia.Shared.LoginGrpc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Client.ViewModels
{
    public partial class ChatsViewModel : ViewModelBase
    {
        private readonly MainViewModel _parent;
        private readonly IFriendService _friendService;
        private Timer _updateTimer;
        private readonly IServiceProvider _services;
        private readonly ISecureStorage _secureStorage;

        public ChatsViewModel(IServiceProvider services, MainViewModel parent, IFriendService chatsService,
            ISecureStorage secureStorage)
        {
            _services = services;
            _parent = parent;
            _friendService = chatsService;
            LoadFriendsAsync();
            StartAutoUpdate();
            _secureStorage = secureStorage;

            _secureStorage.SaveAsync("login", Client.Models.UserSelf.login);
            _secureStorage.SaveAsync("password", Client.Models.UserSelf.password);
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.WriteLine(UserSelf.login);
            Console.WriteLine(UserSelf.password);
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
        [RelayCommand]
        private async Task ClickBackBtn()
        {
            await _secureStorage.SaveAsync("login", "");
            await _secureStorage.SaveAsync("password", "");
            var vm = ActivatorUtilities.CreateInstance<RegisterViewModel>(_services, _parent);
            _parent.CurrentView = vm;
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
