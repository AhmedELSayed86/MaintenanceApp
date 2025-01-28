using NLog;
using System;
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
            try
            {
                // تحميل تكوين NLog من الملف
                LogManager.Setup().LoadConfigurationFromFile("NLog.config");

                var logger = LogManager.GetCurrentClassLogger();
                logger.Info("التطبيق بدأ التشغيل");

                // فتح النافذة الرئيسية
                // var mainWindow = new MainWindow();
                // mainWindow.Show();
            }
            catch(Exception ex)
            {
                // تسجيل الخطأ باستخدام NLog
                LogManager.GetCurrentClassLogger().Error(ex ,"فشل في بدء تشغيل التطبيق");

                // عرض رسالة للمستخدم
                MessageBox.Show("فشل في بدء تشغيل التطبيق. الرجاء التحقق من السجلات للحصول على التفاصيل.");
            }
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
