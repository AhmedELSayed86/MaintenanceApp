using MaintenanceApp.WPF.ViewModels;
using System.Windows;

namespace MaintenanceApp.WPF.Views
{
    /// <summary>
    /// Interaction logic for ImportExcelWindow.xaml
    /// </summary>
    public partial class ImportExcelWindow : Window
    {
        public ImportExcelWindow()
        {
            InitializeComponent();
            DataContext = new ImportExcelViewModel(() => Close());
        }
    }
}
