using Android.Content;
using Client.Services;
using Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Android.Services
{
    public static class AndroidServiceRegistration
    {
        public static IServiceCollection AddAndroidServices(this IServiceCollection services, Context context)
        {
            services.AddTransient<MainViewModel>();
            // android-специфичные сервисы
            services.AddSingleton<IMicrophoneService, AndroidMicrophoneService>();
            services.AddSingleton<IAudioPlayerService>(_ => new AndroidAudioPlayerService(context));
            services.AddSingleton<ISecureStorage>(_ => new AndroidSecureStorage(context));

            return services;
        }
    }
}
