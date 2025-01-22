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
        DataContext = new AddVisitDataModel(new DatabaseHelper()); 
    }
}
