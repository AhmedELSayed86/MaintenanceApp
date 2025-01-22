using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MaintenanceApp.WPF.Views
{
    /// <summary>
    /// Interaction logic for EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : UserControl
    {
        public EmployeeWindow()
        {
            InitializeComponent();
            DataContext = new EmployeeDataViewModel(new DatabaseHelper());
        }
    }
}
