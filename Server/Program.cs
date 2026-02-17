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
});

var app = builder.Build();
app.MapGrpcService<RegisterService>();
app.MapGrpcService<LoginService>();
app.Run();