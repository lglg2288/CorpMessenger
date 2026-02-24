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
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
    [ObservableProperty]
    private object? currentView;

    public MainViewModel()
    {
        CurrentView = null;        
        CurrentView = new RegisterViewModel(this,
            new Interprice.GrpcLoginService(CurrentGrpcChannel.channel),
            new Interprice.GrpcRegisterService(Models.CurrentGrpcChannel.channel));
    }

    [RelayCommand]
    private void GoToRegister()
    {
        CurrentView = null;
        CurrentView = new RegisterViewModel( this,
            new Interprice.GrpcLoginService(CurrentGrpcChannel.channel),
            new Interprice.GrpcRegisterService(Models.CurrentGrpcChannel.channel));
    }

    [RelayCommand]
    private async Task GoToHome()
    {
        return;
    }
}