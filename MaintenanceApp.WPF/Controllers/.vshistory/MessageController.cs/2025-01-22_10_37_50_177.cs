using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MaintenanceApp.WPF.Controllers;

public static class MessageController
{
    private static async Task ResetSummary()
    {
        var SummaryTextBlock = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        SummaryTextBlock.SummaryTextBlock.Text = "";
        SummaryTextBlock.SummaryTextBlock.Foreground = Brushes.White;
        SummaryTextBlock.SummaryBorder.BorderBrush = Brushes.Green;
        await Task.Run(() => Console.WriteLine("message"));
    }

    public static async Task SummaryAsync(string summary)
    {
        ResetSummary();
        var SummaryTextBlock = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        SummaryTextBlock.SummaryTextBlock.Text = summary;
    }

    public static async Task SummaryAsync(string summary ,Brush ForgroundColor)
    {
        ResetSummary();
        var SummaryTextBlock = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        SummaryTextBlock.SummaryTextBlock.Text = summary;
        SummaryTextBlock.SummaryTextBlock.Foreground = ForgroundColor;
    }

    public static async Task SummaryAsync(string summary ,Brush ForgroundColor ,Brush BorderColor)
    {
        ResetSummary();
        var SummaryTextBlock = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault() ?? new MainWindow();
        SummaryTextBlock.SummaryTextBlock.Text = summary;
        SummaryTextBlock.SummaryTextBlock.Foreground = ForgroundColor;
        SummaryTextBlock.SummaryBorder.BorderBrush = BorderColor;
    } 
}
