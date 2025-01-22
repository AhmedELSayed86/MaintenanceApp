using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System.Windows.Controls;

namespace MaintenanceApp.WPF.Views
{
    /// <summary>
    /// Interaction logic for DistributionToTechniciansWindow.xaml
    /// </summary>
    public partial class DistributionToTechniciansWindow : UserControl
    {
        public DistributionToTechniciansWindow()
        {
            InitializeComponent();
            DataContext = new DistributionToTechniciansViewModel(new DatabaseHelper());
        }
    }
}
