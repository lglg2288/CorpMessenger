using Microsoft.AspNetCore.Server.Kestrel.Core;
using MessengerAvalonia.Shared.RegisterGrpc;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5203, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
    options.ListenAnyIP(5204, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps("/app/certs/server.pfx", "YourSecurePassword");
    });
});

var app = builder.Build();
app.MapGrpcService<RegisterService>();
app.MapGrpcService<LoginService>();
app.MapGrpcService<FriendService>();
app.MapGrpcService<ChatService>();
app.Run();