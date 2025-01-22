using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System.Windows.Controls;

namespace MaintenanceApp.WPF.Views
{
    /// <summary>
    /// Interaction logic for Excel_ImporterWindow.xaml
    /// </summary>
    public partial class Excel_ImporterWindow : UserControl
    {
        public Excel_ImporterWindow()
        {
            InitializeComponent();
            DataContext = new Excel_ImporterViewModel(new DatabaseHelper());
        }
    }
}
