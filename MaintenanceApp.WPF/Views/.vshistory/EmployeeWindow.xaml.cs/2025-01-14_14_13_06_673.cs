using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MaintenanceApp.WPF.Views
{
    /// <summary>
    /// Interaction logic for StatusWindow.xaml
    /// </summary>
    public partial class StatusWindow : UserControl
    {
        public StatusWindow()
        {
            InitializeComponent();
            DataContext = new StatusDataViewModel(new DatabaseHelper());
        }
    }
}
