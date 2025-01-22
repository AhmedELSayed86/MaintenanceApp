using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaintenanceApp.WPF.Controllers;

public class Controller
{
    public static  void SummaryAsync(string summary)
    {
        var f = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        f.txtSummary.Text = summary;
    }
}
