using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using MaintenanceApp.WPF.Views;
using Prism.Ioc;
using System.Windows;
using Unity;
namespace MaintenanceApp.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IUnityContainer Container { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // إعداد Unity Container
        Container = new UnityContainer();

        // تسجيل الخدمات
        Container.RegisterSingleton<IDatabaseHelper ,DatabaseHelper>();
        Container.RegisterType<SAPDataViewModel>();

        //// عرض النافذة الرئيسية
        //var mainWindow = new MainWindow
        //{
        //    DataContext = Container.Resolve<SAPDataViewModel>()
        //};
        //mainWindow.Show();
    }
}