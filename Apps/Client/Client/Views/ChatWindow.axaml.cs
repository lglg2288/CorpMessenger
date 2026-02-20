using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Layout;
using Grpc.Net.Client;
using MessengerAvalonia.Shared.ChatsGrpc;
using System;
using System.Net.Http;
using Avalonia.Threading;
using System.Timers;

namespace Client;

public partial class ChatWindow : Window
{
    private Timer _updateTimer;
    public ChatWindow()
    {
        InitializeComponent();
        LoadMessages();
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
                LoadMessages();
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

    //timer^^^----------------------------------------------------------------------
    void LoadMessages()
    {
        ChatTitleTextBlock.Text = $"Чат с {Models.CurrentChat.friendLogin}";


        var channel = GrpcChannel.ForAddress("http://localhost:5203", new GrpcChannelOptions
        {
            HttpHandler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true
            }
        });
        var client = new ChatsService.ChatsServiceClient(channel);
        var reply = client.GetMessages(
            new GetChatsRequest
            {
                UserLogin = Models.UserSelf.login,
                FriendLogin = Models.CurrentChat.friendLogin
            });
        if (reply.Message.Count == 0)
        {
            MessagesStackPanel.Children.Clear();
            MessagesStackPanel.Children.Add(new TextBlock() { Text = "У вас нет сообщений" });
            return;
        }
        MessagesStackPanel.Children.Clear();

        for (int i = 0; i < reply.Message.Count; i++)
        {
            string message = reply.Message[i];
            string writer = reply.Writer[i];

            bool isMe = writer == Models.UserSelf.login;

            MessagesStackPanel.Children.Add(CreateMessageBubble(writer, message, isMe));
        }
        return;
    }

    private void SendButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5203", new GrpcChannelOptions
        {
            HttpHandler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true
            }
        });
        var client = new ChatsService.ChatsServiceClient(channel);
        var reply = client.SendMessage(
            new AddMessageRequest
            {
                UserLogin = Models.UserSelf.login,
                FriendLogin = Models.CurrentChat.friendLogin,
                Text = MessageTextBox.Text
            });
        if (reply.Success)
            LoadMessages();
        else
            MessagesStackPanel.Children.Add(new TextBlock() { Text = reply.Message });
        return;
    }

    private Control CreateMessageBubble(string writer, string message, bool isMe)
    {
        var border = new Border
        {
            Background = isMe
                ? new SolidColorBrush(Color.Parse("#2D2D2D"))
                : new SolidColorBrush(Color.Parse("#3D3D3D")),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(10),
            HorizontalAlignment = isMe ? HorizontalAlignment.Left : HorizontalAlignment.Right,
            MaxWidth = 500
        };

        var stack = new StackPanel();

        var writerBlock = new TextBlock
        {
            Text = isMe ? "Вы" : writer,
            FontWeight = FontWeight.Bold,
            Foreground = isMe
                ? new SolidColorBrush(Color.Parse("#4A9EFF"))
                : new SolidColorBrush(Color.Parse("#FF6B6B"))
        };

        var messageBlock = new TextBlock
        {
            Text = message,
            Foreground = Brushes.White,
            TextWrapping = TextWrapping.Wrap
        };

        var timeBlock = new TextBlock
        {
            Text = DateTime.Now.ToString("HH:mm"), // временно
            FontSize = 11,
            Foreground = new SolidColorBrush(Color.Parse("#808080")),
            HorizontalAlignment = HorizontalAlignment.Right
        };

        stack.Children.Add(writerBlock);
        stack.Children.Add(messageBlock);
        stack.Children.Add(timeBlock);

        border.Child = stack;

        return border;
    }
}