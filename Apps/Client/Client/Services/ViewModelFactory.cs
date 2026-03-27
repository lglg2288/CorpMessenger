using Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Services
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IServiceProvider _provider;

        public ViewModelFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public T Create<T>() where T : ViewModelBase
        {
            return _provider.GetRequiredService<T>();
        }

        public T Create<T>(params object[] parameters) where T : ViewModelBase
        {
            // Для случаев, когда VM требует доп. параметры в конструкторе
            // Но лучше стараться избегать → передавать через свойства или методы
            throw new NotSupportedException("Use parameterless ctor + Init method");
        }
    }
}
