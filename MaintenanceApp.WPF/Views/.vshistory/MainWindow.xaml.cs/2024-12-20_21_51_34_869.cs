using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System.Windows;

namespace MaintenanceApp.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IDatabaseHelper databaseHelper;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(databaseHelper);
            ControlHelper.WindowActivate(this);
        }

        private void Window_Loaded(object sender ,RoutedEventArgs e)
        {
            ControlHelper.SetScreenResolution(this);
        }
    }
}