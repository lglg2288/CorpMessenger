using Client.Services;
using Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Desktop.Services
{
    public static class DesktopServiceRegistration
    {
        public static IServiceCollection AddDesktopServices(this IServiceCollection services)
        {
            services.AddSingleton<IMicrophoneService, DesktopMicrophoneService>();
            services.AddSingleton<ISecureStorage, DesktopSecureStorage>();

            return services;
        }
    }
}
