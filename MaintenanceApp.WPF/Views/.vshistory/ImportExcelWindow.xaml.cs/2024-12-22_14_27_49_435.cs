using MaintenanceApp.WPF.ViewModels;
using System.Windows.Controls;

namespace MaintenanceApp.WPF.Views;

/// <summary>
/// Interaction logic for ImportExcelWindow.xaml
/// </summary>
public partial class ImportExcelWindow : UserControl
{
    ImportExcelViewModel viewModel;
    public ImportExcelWindow()
    {
        viewModel = new ImportExcelViewModel();
        InitializeComponent();
        DataContext = viewModel;
    }
}
