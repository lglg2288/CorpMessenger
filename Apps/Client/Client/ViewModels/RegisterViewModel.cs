using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        public RegisterViewModel(MainViewModel parent, ILoginService loginService)
        {
            _parent = parent;
            _loginService = loginService;
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
