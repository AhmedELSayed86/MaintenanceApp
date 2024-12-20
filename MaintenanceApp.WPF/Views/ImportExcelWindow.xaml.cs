using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System.Windows;

namespace MaintenanceApp.WPF.Views
{
    /// <summary>
    /// Interaction logic for ImportExcelWindow.xaml
    /// </summary>
    public partial class ImportExcelWindow : Window
    {
        public ImportExcelWindow(IDatabaseHelper databaseHelper)
        {
            InitializeComponent();
            DataContext = new ImportExcelViewModel(databaseHelper,() => Close());
        }
    }
}
