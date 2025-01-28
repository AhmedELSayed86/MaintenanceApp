using NLog;
using System;
using System.IO;
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
            { // تحديد مسار قاعدة البيانات
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string databasePath = Path.Combine(documentsPath ,"MaintenanceApp.db3");

                // تسجيل المسار الديناميكي في المتغير
                LogManager.Configuration.Variables["DbPath"] = databasePath;

                // تسجيل المسار في حالة وجود خطأ
                Console.WriteLine($"Database Path: {databasePath}");

                // تحميل تكوين NLog
                LogManager.Setup().LoadConfigurationFromFile("NLog.config");

                // التأكد من أن التكوين تم بنجاح
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
