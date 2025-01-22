using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Views; 
using System.Linq;
using System.Windows; 

namespace MaintenanceApp.WPF.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IWindowService _windowService;
    private readonly IUnityContainer _container;
    private static MainWindow mainWindow;

    public DelegateCommand ApplicationShutdownCommand { get; }
    public DelegateCommand OpenPrintWindowCommand { get; }
    public DelegateCommand OpenAddDataWindowCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand OpeneImplementedCommand { get; }
    public DelegateCommand OpeneEmployeeCommand { get; }
    public DelegateCommand OpeneImportExcelNotificationsCommand { get; }


    public MainViewModel(IWindowService windowService ,IUnityContainer container)
    {
        _windowService = windowService;
        _container = container;

        ApplicationShutdownCommand = new DelegateCommand(ShutdownApplication);
        OpenPrintWindowCommand = new DelegateCommand(OpenPrintWindow);
        OpenAddDataWindowCommand = new DelegateCommand(OpenAddDataWindow);
        OpeneImplementedCommand = new DelegateCommand(OpeneImplementedWindow);
        OpeneEmployeeCommand = new DelegateCommand(OpeneEmployeeWindow);
        OpeneImportExcelNotificationsCommand = new DelegateCommand(OpeneImportExcelNotificationsWindow);

        GetBasePage();
    }

    private void ShutdownApplication()
    {
        var result = MessageBox.Show("هل تريد الخروج من البرنامج؟" ,"تنبيه" ,
            MessageBoxButton.YesNo ,MessageBoxImage.Question ,MessageBoxResult.No ,MessageBoxOptions.RtlReading);

        if(result == MessageBoxResult.Yes)
        {
            Application.Current.Shutdown();
        }
    }

    private void OpenPrintWindow()
    {
        _windowService.ShowDialog<PrintViewModel>(() =>
        {
            // يتم استدعاء الـ Action عند غلق النافذة
            // ضع الكود الذي يجب تنفيذه هنا عند إغلاق النافذة
        });

    }

    private void OpenAddDataWindow()
    {
        _windowService.ShowDialog<AddDataViewModel>();
    }

    private void OpeneImplementedWindow()
    {
        AddDataWindow addDataWindow = new();
        mainWindow.MainContentControl.Content = addDataWindow;
    }

    private void OpeneImportExcelNotificationsWindow()
    {
        ImportExcelWindow employeeWindow = _container.Resolve<ImportExcelWindow>();
        employeeWindow.Show();
    }

    private void OpeneEmployeeWindow()
    {
        EmployeeWindow employeeWindow = _container.Resolve<EmployeeWindow>();
        employeeWindow.Show();
    }

    public static void GetBasePage()
    {
        mainWindow = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
    }
}
