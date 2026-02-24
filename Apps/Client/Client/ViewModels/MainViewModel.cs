using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Net.Client;
using MessengerAvalonia.Shared.LoginGrpc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
    [ObservableProperty]
    private object? currentView;

    public MainViewModel()
    {
    }

    [RelayCommand]
    private void GoToRegister()
    {
        CurrentView = null;
        var channel = GrpcChannel.ForAddress("http://neogus.ru:5203", new GrpcChannelOptions
        {
        });
        Interprice.GrpcLoginService client = new Interprice.GrpcLoginService(channel);

        CurrentView = new RegisterViewModel(this, client);
    }

    [RelayCommand]
    private async Task GoToHome()
    {
        return;
    }
}