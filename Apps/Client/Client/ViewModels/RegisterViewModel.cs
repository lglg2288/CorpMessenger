using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessengerAvalonia.Shared.ChatsGrpc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly MainViewModel _parent;
        private readonly ILoginService _loginService;
        private readonly IRegisterService _registerService;
        private readonly IServiceProvider _services;
        public RegisterViewModel(IServiceProvider services, MainViewModel parent, ILoginService loginService, IRegisterService registerService)
        {
            _services = services;
            _parent = parent;
            _loginService = loginService;
            _registerService = registerService;
        }

        [ObservableProperty]
        private string? login;
        [ObservableProperty]
        private string? password;
        [ObservableProperty]
        private string? errorMessage;

        [RelayCommand]
        private async Task LoginAsync()
        {
            try
            {
                var response = await _loginService.SignInAsync(Login, Password);

                if (response.Success)
                {
                    Client.Models.UserSelf.login = Login;
                    Client.Models.UserSelf.password = Password;
                    var vm = ActivatorUtilities.CreateInstance<ChatsViewModel>(_services, _parent);
                    _parent.CurrentView = vm;
                }
                else
                {
                    ErrorMessage = response.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Network error: {ex.Message}";
            }
        }
        [RelayCommand]
        private async Task RegisterAsync()
        {
            try
            {
                var response = await _registerService.RegisterAsync(Login, Password);

                if (response.Success)
                {
                    ErrorMessage = response.Message;
                }
                else
                {
                    ErrorMessage = response.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Network error: {ex.Message}";
            }
        }
    }
}
