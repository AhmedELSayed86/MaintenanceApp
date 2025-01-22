using System.Linq;
using System.Windows;

namespace MaintenanceApp.WPF.Controllers;

public class MessageController
{
    public static void SummaryAsync(string summary)
    {
        var f = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        f.txtSummary.Text = summary;
    }
}
