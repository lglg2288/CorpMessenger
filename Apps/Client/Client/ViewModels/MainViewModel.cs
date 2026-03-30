using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Net.Client;
using MessengerAvalonia.Shared.LoginGrpc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Client.Models;
using Client.Services;

namespace Client.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IServiceProvider _services;
    private readonly ISecureStorage _secureStorage;


    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
    [ObservableProperty]
    private object? currentView;

    public MainViewModel(IServiceProvider services, ISecureStorage secureStorage)
    {
        _services = services;
        _secureStorage = secureStorage;
        CurrentView = null;
    }

    public async Task InitializeAsync()
    {
        try
        {
            UserSelf.login = await _secureStorage.GetAsync("login");
            UserSelf.password = await _secureStorage.GetAsync("password");
            Console.WriteLine("##########################################################################################################################");
            if (string.IsNullOrWhiteSpace(UserSelf.login) || string.IsNullOrWhiteSpace(UserSelf.password))
                throw new Exception("No credentials found");
            var vm = ActivatorUtilities.CreateInstance<ChatsViewModel>(_services, this);
            CurrentView = vm;
        }
        catch
        {
            CurrentView = new RegisterViewModel(
                _services,
                this,
                new Interprice.GrpcLoginService(CurrentGrpcChannel.channel),
                new Interprice.GrpcRegisterService(Models.CurrentGrpcChannel.channel));
        }
    }

    [RelayCommand]
    private void GoToRegister()
    {
        var vm = ActivatorUtilities.CreateInstance<RegisterViewModel>(_services);
        CurrentView = vm;
    }

    [RelayCommand]
    private async Task GoToHome()
    {
        return;
    }
}