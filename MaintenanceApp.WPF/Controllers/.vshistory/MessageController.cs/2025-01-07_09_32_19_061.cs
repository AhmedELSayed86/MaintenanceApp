using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MaintenanceApp.WPF.Controllers;

public class MessageController
{
    public static void SummaryAsync(string summary)
    {
        var SummaryTextBlock = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        SummaryTextBlock.txtSummary.Text = summary;
    }

    public static void SummaryAsync(string summary ,Brush BackgroundColor)
    {
        var SummaryTextBlock = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        SummaryTextBlock.txtSummary.Text = summary;
    }

    public static void SummaryAsync(string summary ,Brush BackgroundColor ,Brush ForgroundColor)
    {
        var SummaryTextBlock = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        SummaryTextBlock.txtSummary.Text = summary;
    }
}
