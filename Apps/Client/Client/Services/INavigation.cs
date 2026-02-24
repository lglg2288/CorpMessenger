using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Services
{
    public interface INavigation
    {
        void NavigateTo<TViewModel>() where TViewModel : ObservableObject;
    }
}
