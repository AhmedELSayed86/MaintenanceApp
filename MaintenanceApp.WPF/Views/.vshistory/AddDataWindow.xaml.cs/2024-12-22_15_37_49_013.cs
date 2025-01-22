using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System.Windows.Controls;

namespace MaintenanceApp.WPF.Views
{
    /// <summary>
    /// Interaction logic for AddDataWindow.xaml
    /// </summary>
    public partial class AddDataWindow : UserControl
    {
        public AddDataWindow()
        {
            InitializeComponent();
            var viewModel = new AddDataViewModel(new DatabaseHelper());
            DataContext = viewModel;
        }
    }
}
