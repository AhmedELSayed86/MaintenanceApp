using MaintenanceApp.WPF.Views;
using System.Linq;
using System.Windows;

namespace MaintenanceApp.WPF.Helper;

public static class ControlHelper
{
    public static void WindowActivate(Window window ,bool OnTop = false)
    {
        window = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        #region تثبيت التطبيق في المقدمة
        window.Dispatcher.Invoke(() =>
        {
            window.WindowState = System.Windows.WindowState.Normal;
            window.Topmost = OnTop;
            window.Activate();
            window.Focus();
        });
        #endregion
    }

    public static void SetScreenResolution(Window window)
    {
        // احصل على دقة الشاشة
        var screenWidth = SystemParameters.WorkArea.Width;
        var screenHeight = SystemParameters.WorkArea.Height;

        // تعيين ارتفاع النافذة ليكون بملء الشاشة مع مراعاة شريط المهام
        window.Height = screenHeight;

        // تعيين عرض النافذة إلى 400 بكسل
        window.Width = screenWidth;

        // تعيين موقع النافذة إلى أقصى يمين الشاشة
        window.Left = 0;
        window.Top = 0;
    }
}