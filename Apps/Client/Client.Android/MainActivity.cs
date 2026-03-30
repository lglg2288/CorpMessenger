using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Client.Android.Services;
using Microsoft.Extensions.DependencyInjection;
using Client.Services;

namespace Client.Android;

[Activity(
    Label = "Client.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        var service = new ServiceCollection()
            .AddAppServices()
            .AddAndroidServices(this);
        
        App.services = service.BuildServiceProvider();

        base.OnCreate(savedInstanceState);
    }
}
