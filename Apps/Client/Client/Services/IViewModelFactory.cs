using Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Services
{
    public interface IViewModelFactory
    {
        T Create<T>() where T : ViewModelBase;
        T Create<T>(params object[] parameters) where T : ViewModelBase; // если нужны параметры
    }
}
