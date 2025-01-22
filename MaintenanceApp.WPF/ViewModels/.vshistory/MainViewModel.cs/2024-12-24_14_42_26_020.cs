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
    public ICommand OpenHomeCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand PrintCommand { get; }
    public ICommand OpeneEmployeeCommand { get; }
    public ICommand OpenPrintCommand { get; }
    public ICommand OpeneImplementedCommand { get; }
    public ICommand OpeneImportExcelNotificationsCommand { get; }
    public ICommand ApplicationShutdownCommand { get; }

    public MainViewModel()
    {
        OpenHomeCommand = new RelayCommand(o => OpenHomeWindow());
        OpeneEmployeeCommand = new RelayCommand(o => OpeneEmployeeWindow());
        OpenPrintCommand = new RelayCommand(o => OpenPrintWindow());
        OpeneImportExcelNotificationsCommand = new RelayCommand(o => OpeneImportExcelNotificationsWindow());
        ApplicationShutdownCommand = new RelayCommand(o => ShutdownApplication());
        OpeneImplementedCommand = new RelayCommand(o => OpeneImplementedWindow());
        GetBasePage();
        OpenHomeCommand.Execute(1);
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

    private void OpenHomeWindow()
    {
        HomeWindow homeWindow = System.Windows.Application.Current.Windows.OfType<HomeWindow>().FirstOrDefault() ?? new HomeWindow();
        mainWindow.MainContentControl.Content = homeWindow;
    }

    private void OpenPrintWindow()
    {
        PrintWindow printWindow = System.Windows.Application.Current.Windows.OfType<PrintWindow>().FirstOrDefault() ?? new PrintWindow();
        mainWindow.MainContentControl.Content = printWindow;
    }

    private void OpenAddDataWindow()
    {
        _windowService.ShowDialog<AddDataViewModel>();
    }

    private void OpeneImplementedWindow()
    {
        AddDataWindow addDataWindow = System.Windows.Application.Current.Windows.OfType<AddDataWindow>().FirstOrDefault() ?? new AddDataWindow();
        mainWindow.MainContentControl.Content = addDataWindow;
    }

    private void OpeneImportExcelNotificationsWindow()
    {
        ImportExcelWindow excelWindow = System.Windows.Application.Current.Windows.OfType<ImportExcelWindow>().FirstOrDefault() ?? new ImportExcelWindow();

        mainWindow.MainContentControl.Content = excelWindow;
    }

    private void OpeneEmployeeWindow()
    {
        EmployeeWindow employeeWindow = System.Windows.Application.Current.Windows.OfType<EmployeeWindow>().FirstOrDefault() ?? new EmployeeWindow();
        mainWindow.MainContentControl.Content = employeeWindow;
    }

    public static void GetBasePage()
    {
        mainWindow = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
    }
}
