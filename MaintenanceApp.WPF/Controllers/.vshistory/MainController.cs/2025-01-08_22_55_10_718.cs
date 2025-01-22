using MaintenanceApp.WPF.Views;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MaintenanceApp.WPF.Controllers;

public class MainController
{
    private static void ResetSummary()
    {
        var SummaryTextBlock = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        SummaryTextBlock.SummaryTextBlock.Text = "";
        SummaryTextBlock.SummaryTextBlock.Foreground = Brushes.White;
        SummaryTextBlock.SummaryBorder.BorderBrush = Brushes.Green;
    }

    public static void MangVisitDateBox()
    {
        ResetSummary();
        var SummaryTextBlock = Application.Current.Windows.OfType<AddVisitDataWindow>().FirstOrDefault() ?? new AddVisitDataWindow();
        SummaryTextBlock.VisitDateBox.Focus();
        SummaryTextBlock.VisitDateBox.Background= Brushes.Red;
        SummaryTextBlock.VisitDateBox.Foreground= Brushes.IndianRed;
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
}
