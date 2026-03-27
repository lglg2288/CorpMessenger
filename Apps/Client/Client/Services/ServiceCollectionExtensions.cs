using Client.Interprice;
using Client.ViewModels;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Client.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            // Общие сервисы (если есть)
            // services.AddSingleton<SomeSharedService>();

            // ViewModels — обычно Transient или Scoped
            services.AddTransient<MainViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<ChatViewModel>();
            services.AddTransient<ChatsViewModel>();

            // Другие общие вещи...
            services.AddSingleton(sp => Models.CurrentGrpcChannel.channel);
            services.AddSingleton<ILoginService, GrpcLoginService>();
            services.AddSingleton<IRegisterService, GrpcRegisterService>();
            services.AddSingleton<IFriendService, GrpcFriendService>();
            services.AddSingleton<IChatsService, GrpcChatsService>();
            services.AddSingleton<ICallService, GrpcCallService>();

            return services;
        }
    }
}
