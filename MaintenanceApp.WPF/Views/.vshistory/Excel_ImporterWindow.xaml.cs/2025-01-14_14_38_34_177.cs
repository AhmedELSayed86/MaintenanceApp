using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MaintenanceApp.WPF.Views
{
    /// <summary>
    /// Interaction logic for Excel_ImporterWindow.xaml
    /// </summary>
    public partial class Excel_ImporterWindow : UserControl
    {
        public Excel_ImporterWindow()
        {
            InitializeComponent();
            DataContext = new Excel_ImporterViewModel(new DatabaseHelper());
        }
    }
}
