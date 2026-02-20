using Avalonia.Controls;
using System.Threading.Tasks;

namespace Client.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            new RegisterWindow().Show();
        }
    }
}