using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace MaintenanceApp.WPF.Controllers;

public static class MessageController
{
    private static Task ResetSummaryAsync()
    {
        try
        {
            Debug.WriteLine("جاري تنفيذ ResetSummaryAsync...");
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            Debug.WriteLine(mainWindow == null ? "النافذة الرئيسية غير موجودة." : "تم العثور على النافذة الرئيسية.");
             
            if(mainWindow != null)
            {
                  Application.Current.Dispatcher.Invoke(() =>
                {
                      mainWindow.SummaryTextBlock.Text = "";
                    mainWindow.SummaryTextBlock.Foreground = Brushes.White;
                    mainWindow.SummaryBorder.BorderBrush = Brushes.Green;
                });
            }
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message ,"Error:-");
        }

        return Task.CompletedTask;
    }

    public static async Task SummaryAsync(string summary ,Brush Color = null)
    {
        try
        {
            await ResetSummaryAsync();

            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if(mainWindow == null)
            {
                MessageBox.Show("النافذة الرئيسية غير موجودة." ,"خطأ");
                return;
            }

             Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    mainWindow.SummaryTextBlock.Text = summary;
                    if(Color != null)
                    {
                        mainWindow.SummaryTextBlock.Foreground = Color;
                        mainWindow.SummaryBorder.BorderBrush = Color;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"حدث خطأ في تحديث الواجهة: {ex.Message}" ,"خطأ");
                }
           });
        }
        catch(Exception ex)
        {
            MessageBox.Show($"حدث خطأ: {ex.Message}" ,"خطأ");
        }
    }
}
