using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System.Windows.Controls;

namespace MaintenanceApp.WPF.Views;

/// <summary>
/// Interaction logic for ImportExcelWindow.xaml
/// </summary>
public partial class ImportExcelWindow : UserControl
{ 
    public ImportExcelWindow()
    {
      
        InitializeComponent(); 
        var viewModel = new AddDataViewModel(new DatabaseHelper());
        DataContext = viewModel; 
    }
}
