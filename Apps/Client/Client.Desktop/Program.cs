using Avalonia;
using Client.Desktop.Services;
using Client.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Client.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        var services = new ServiceCollection();

        services.AddAppServices()
                .AddDesktopServices();

        var provider = services.BuildServiceProvider();
        App.services = provider;

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
