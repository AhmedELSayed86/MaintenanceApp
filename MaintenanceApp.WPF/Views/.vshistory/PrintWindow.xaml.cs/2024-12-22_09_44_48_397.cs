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
using System.Windows.Shapes;

namespace MaintenanceApp.WPF.Views
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : UserControl
    {
        PrintViewModel viewModel;
        public PrintWindow( )
        {
             viewModel =new PrintViewModel(Close);
            InitializeComponent();

            ControlHelper.WindowActivate(this);

            // تعيين DataContext
            DataContext = viewModel;
        }
    }
}
