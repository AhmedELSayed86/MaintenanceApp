using NLog;
using System.Windows;

namespace MaintenanceApp.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    { 
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // تهيئة NLog
            LogManager.Setup().LoadConfigurationFromFile("NLog.config");

            // تسجيل بدء التشغيل
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("التطبيق بدأ التشغيل");

            // فتح النافذة الرئيسية
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // تسجيل إيقاف التشغيل
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("التطبيق تم إيقافه");

            base.OnExit(e);
        }
    }
}
