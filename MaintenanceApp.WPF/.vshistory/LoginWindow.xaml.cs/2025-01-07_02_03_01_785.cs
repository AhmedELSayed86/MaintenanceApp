using MaintenanceApp.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MaintenanceApp.WPF
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent(); 
            DataContext = new LoginViewModel();
        }
    }
}
