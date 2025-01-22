using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Views; 
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IWindowService _windowService; 
    private static MainWindow mainWindow;
    // Commands
    public ICommand LoadCommand { get; }
    public ICommand PrintCommand { get; }
    public ICommand OpeneEmployeeCommand { get; }
    public ICommand OpenPrintCommand { get; }
    public ICommand OpeneImplementedCommand { get; }
    public ICommand OpeneImportExcelNotificationsCommand { get; }
    public ICommand ApplicationShutdownCommand { get; }

    public MainViewModel(  )
    {
        OpeneEmployeeCommand = new RelayCommand(o=> OpeneEmployeeWindow());
        OpenPrintCommand = new RelayCommand(o=> OpenPrintWindow());
        OpeneImportExcelNotificationsCommand = new RelayCommand(o=> OpeneImportExcelNotificationsWindow());
        ApplicationShutdownCommand = new RelayCommand(o=> ShutdownApplication());
        OpeneImplementedCommand = new RelayCommand(o=> OpeneImplementedWindow());
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
        ImportExcelWindow employeeWindow = System.Windows.Application.Current.Windows.OfType<ImportExcelWindow>().FirstOrDefault(); 
         
        employeeWindow.Show();
    }

    private void OpeneEmployeeWindow()
    { 
        EmployeeWindow employeeWindow = System.Windows.Application.Current.Windows.OfType<EmployeeWindow>().FirstOrDefault(); employeeWindow.Show();
    }

    public static void GetBasePage()
    {
        mainWindow = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
    }
}
