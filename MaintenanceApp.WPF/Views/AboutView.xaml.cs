using MaintenanceApp.WPF.ViewModels;
using System.Windows.Controls;

namespace MaintenanceApp.WPF.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
            DataContext = new AboutViewModel(); // ربط الـ ViewModel بالـ View
        }
    }
}
