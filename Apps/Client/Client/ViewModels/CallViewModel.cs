using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public partial class CallViewModel : ViewModelBase
    {
        private readonly MainViewModel _parent;
        private readonly ICallService _callService;
        private readonly IAudioPlayerService _audioPlayer;
        private readonly IServiceProvider _services;

        [ObservableProperty]
        private string friendName = CurrentChat.friendLogin ?? string.Empty;

        [ObservableProperty]
        private string callStatus = "Соединение...";

        public CallViewModel(IServiceProvider services, MainViewModel parent, ICallService callService, IAudioPlayerService audioPlayer)
        {
            _services = services;
            _parent = parent;
            _callService = callService;
            _audioPlayer = audioPlayer;

            StartCallAsync();
        }

        private async void StartCallAsync()
        {
            try
            {
                _audioPlayer.Start();
                _callService.onAudioChunkReceived += (byte[] bytes) =>
                {
                    _audioPlayer.PlayChunk(bytes);
                };
                CallStatus = "Звонок...";
                await _callService.StartCallAsync(UserSelf.login!, CurrentChat.friendLogin!);
                CallStatus = "Звонок завершён";
            }
            catch (Exception)
            {
                CallStatus = "Соединение разорвано";
            }
        }

        [RelayCommand]
        private async Task HangUp()
        {
            try
            {
                await _callService.EndCallAsync();
                _audioPlayer.Stop();
            }
            catch { }
            finally
            {
                var vm = ActivatorUtilities.CreateInstance<ChatViewModel>(_services, _parent);
                _parent.CurrentView = vm;
            }
        }
    }
}
