using DocumentFormat.OpenXml.Office2010.Excel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MaintenanceApp.WPF.Controllers;

public class MessageController
{
    public static void SummaryAsync(string summary)
    {
        ResetSummary();
        var SummaryTextBlock = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        SummaryTextBlock.SummaryTextBlock.Text = summary;
    }

    public static void SummaryAsync(string summary ,Brush ForgroundColor)
    {
        ResetSummary();
        var SummaryTextBlock = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        SummaryTextBlock.SummaryTextBlock.Text = summary;
        SummaryTextBlock.SummaryTextBlock.Foreground = ForgroundColor;
    }

    public static void SummaryAsync(string summary ,Brush ForgroundColor ,Brush BorderColor)
    {
        ResetSummary();
        var SummaryTextBlock = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        SummaryTextBlock.SummaryTextBlock.Text = summary;
        SummaryTextBlock.SummaryTextBlock.Foreground = ForgroundColor;
        SummaryTextBlock.SummaryBorder.BorderBrush = BorderColor;
    }

    private static void ResetSummary()
    {
        var SummaryTextBlock = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        SummaryTextBlock.SummaryTextBlock.Text = "";
        SummaryTextBlock.SummaryTextBlock.Foreground = Brushes.White;
        SummaryTextBlock.SummaryBorder.BorderBrush = Brushes.Green;
    }
}
