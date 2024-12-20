using MaintenanceApp.WPF.ViewModels;
using System.Windows;

namespace MaintenanceApp.WPF.Views
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        public PrintWindow()
        {
            InitializeComponent();
            DataContext = new PrintViewModel(() => Close());
        }
    }
}
