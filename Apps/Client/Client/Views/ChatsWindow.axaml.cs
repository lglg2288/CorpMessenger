using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Grpc.Net.Client;
using MessengerAvalonia.Shared.FriendsGrpc;
using System;
using System.Net.Http;
using Avalonia.Threading;
using System.Timers;

namespace Client;

public partial class ChatsWindow : Window
{
    private Timer _updateTimer;

    public ChatsWindow()
    {
        InitializeComponent();
        ChatsTitle.Text = $"Привет {Models.UserSelf.login}!";
        LoadFriends();
        StartAutoUpdate();
    }

    private void StartAutoUpdate()
    {
        _updateTimer = new Timer(3000);
        _updateTimer.Elapsed += async (sender, e) =>
        {
            // Используем Dispatcher.UIThread для обновления UI из другого потока
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                LoadFriends();
            });
        };
        _updateTimer.AutoReset = true;
        _updateTimer.Start();
    }

    protected override void OnClosed(EventArgs e)
    {
        _updateTimer?.Stop();
        _updateTimer?.Dispose();
        base.OnClosed(e);
    }

    //timer^^^----------------------------------------------------------------------------------

    private void AddFriendButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5203", new GrpcChannelOptions
        {
            HttpHandler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true
            }
        });
        var client = new FriendsService.FriendsServiceClient(channel);

        var reply = client.AddFriend(
            new AddFriendRequest
            {
                UserLogin = Models.UserSelf.login,
                FriendLogin = FriendsLoginTextBox.Text
            });
        ChatsTitle.Text = reply.Message;
        LoadFriends();
    }

    private void LoadFriends()
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5203", new GrpcChannelOptions
        {
            HttpHandler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true
            }
        });
        var client = new FriendsService.FriendsServiceClient(channel);
        var reply = client.GetFriends(new FriendsRequest { Login = Models.UserSelf.login });
        if (reply.Friends.Count == 0)
        {
            ChatsTitle.Text = "У вас нет друзей";
            return;
        }
        ChatsStackPanel.Children.Clear();
        foreach (var friend in reply.Friends)
        {
            ChatsStackPanel.Children.Add(CreateChatButton(friend));
        }
        return;
    }
    private Button CreateChatButton(string friend)
    {
        var btn = new Button() { Content = friend };
        btn.Click += ChatButton_Click;
        return btn;
    }
    private void ChatButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            Models.CurrentChat.friendLogin = button.Content.ToString();
            new ChatWindow().Show();
            this.Close();
        }
    }
}