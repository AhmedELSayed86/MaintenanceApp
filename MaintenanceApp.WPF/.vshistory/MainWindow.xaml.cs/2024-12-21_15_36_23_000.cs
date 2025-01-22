using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System.Windows;

namespace MaintenanceApp.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            ControlHelper.WindowActivate(this);

            // تعيين DataContext
            DataContext = viewModel;
        }

        private void Window_Loaded(object sender ,RoutedEventArgs e)
        {
            ControlHelper.SetScreenResolution(this);
        }
    }
}