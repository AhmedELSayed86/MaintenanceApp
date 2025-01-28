using NLog;
using System.Windows;

namespace MaintenanceApp.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // تهيئة NLog
            LogManager.LoadConfiguration("NLog.config");

            // تسجيل بدء التشغيل
            Logger.Info("التطبيق بدأ التشغيل");

            // فتح النافذة الرئيسية
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // تسجيل إيقاف التشغيل
            Logger.Info("التطبيق تم إيقافه");

            base.OnExit(e);
        }
    }
}
