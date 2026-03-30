using Avalonia.Controls;
using Client.ViewModels;

namespace Client.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            await vm.InitializeAsync();
    }
}