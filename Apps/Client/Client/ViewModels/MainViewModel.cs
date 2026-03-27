using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Net.Client;
using MessengerAvalonia.Shared.LoginGrpc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Client.Models;

namespace Client.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IServiceProvider _services;

    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
    [ObservableProperty]
    private object? currentView;

    public MainViewModel(IServiceProvider services)
    {
        _services = services;
        CurrentView = null;
        CurrentView = new RegisterViewModel(_services, this,
            new Interprice.GrpcLoginService(CurrentGrpcChannel.channel),
            new Interprice.GrpcRegisterService(Models.CurrentGrpcChannel.channel));
        
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