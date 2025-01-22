using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System.Windows.Controls;

namespace MaintenanceApp.WPF.Views;

/// <summary>
/// Interaction logic for AddVisitDataWindow.xaml
/// </summary>
public partial class AddVisitDataWindow : UserControl
{
    public AddVisitDataWindow()
    {
        InitializeComponent();
        var viewModel = new AddDataViewModel(new DatabaseHelper());
        DataContext = viewModel;
    }
}
