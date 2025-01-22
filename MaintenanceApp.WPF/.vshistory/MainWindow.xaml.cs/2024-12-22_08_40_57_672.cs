using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System.Windows;

namespace MaintenanceApp.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    MainViewModel mainViewModel;
    public MainWindow()
    {
        mainViewModel = new MainViewModel();
        InitializeComponent();
        ControlHelper.WindowActivate(this);
        DataContext = mainViewModel;
    }

    private void Window_Loaded(object sender ,RoutedEventArgs e)
    {
        ControlHelper.SetScreenResolution(this);
    }
}