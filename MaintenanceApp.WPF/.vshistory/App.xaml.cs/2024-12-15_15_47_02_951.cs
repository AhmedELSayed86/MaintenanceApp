using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace MaintenanceApp.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // تسجيل الخدمات
        containerRegistry.RegisterSingleton<IDatabaseHelper ,DatabaseHelper>();
        containerRegistry.RegisterForNavigation<MainWindow>();
    }

    protected override Window CreateShell()
    {
        // استدعاء نافذة البداية
        return Prism.Ioc.ContainerLocator.Container.Resolve<MainWindow>();
    }
}