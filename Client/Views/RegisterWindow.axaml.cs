using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Grpc.Net.Client;
using Grpc.Net.Client;
using MessengerAvalonia.Shared;
using Microsoft.VisualBasic;
using System;
using System.Net.Http;

namespace Client;

public partial class RegisterWindow : Window
{
    public RegisterWindow()
    {
        InitializeComponent();
    }

    private void LoginButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5203", new GrpcChannelOptions
        {
            HttpHandler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true
            }
        });
        var client = new Register.RegisterClient(channel);

        var reply = client.SignUp(new RegisterRequest { Login = LoginTextBox.Text, Password = PasswordTextBox.Text });
        InformationTextBlock.Text = reply.Message;
    }
}       