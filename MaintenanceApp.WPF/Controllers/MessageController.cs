using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MaintenanceApp.WPF.Controllers;

public static class MessageController
{
    private static async Task ResetSummaryAsync()
    {
        var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        if(mainWindow != null)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                mainWindow.SummaryTextBlock.Text = "";
                mainWindow.SummaryTextBlock.Foreground = Brushes.White;
                mainWindow.SummaryBorder.BorderBrush = Brushes.Green;
            });
        }
    }

    public static async Task SummaryAsync(string summary ,Brush Color = null )
    {
        await ResetSummaryAsync();

        var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        if(mainWindow != null)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                mainWindow.SummaryTextBlock.Text = summary;
                if(Color != null)
                {
                    mainWindow.SummaryTextBlock.Foreground = Color;              
                    mainWindow.SummaryBorder.BorderBrush = Color;
                }
            });
        }
    }
}
