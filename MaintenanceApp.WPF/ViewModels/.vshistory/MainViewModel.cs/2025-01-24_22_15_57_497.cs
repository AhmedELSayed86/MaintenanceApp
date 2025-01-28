using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Views;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MaintenanceApp.WPF.ViewModels;

public class MainViewModel : BaseViewModel
{
    private static MainWindow _mainWindow;
    private Brush _exitBackground = Brushes.LightGray;

    // الخصائص
    public string CurrentUserName => CurrentUser.Instance.Username;
    public Brush EXITBackground
    {
        get => _exitBackground;
        set => SetProperty(ref _exitBackground ,value);
    }

    private string _summary = "بسم الله";
    public string Summary
    {
        get => _summary;
        set => SetProperty(ref _summary ,value ,onChanged: () => CommandManager.InvalidateRequerySuggested());
    }

    // الأوامر
    public ICommand LogInCommand { get; }
    public ICommand LogOutCommand { get; }
    public ICommand OpenHomeCommand { get; }
    public ICommand OpenAboutCommand { get; }
    public ICommand OpenPrintCommand { get; }
    public ICommand OpenEmployeeCommand { get; }
    public ICommand OpenImplementedCommand { get; }
    public ICommand OpenDistributionToTechniciansCommand { get; }
    public ICommand OpenImportExcelNotificationsCommand { get; }
    public ICommand OpenExcelImporterCommand { get; }
    public ICommand ApplicationShutdownCommand { get; }

    public MainViewModel()
    { 
        // تهيئة الأوامر
        OpenAboutCommand = CreateCommand(OpenAboutWindow);
        OpenHomeCommand = CreateCommand(OpenHomeWindow);
        OpenEmployeeCommand = CreateCommand(OpenEmployeeWindow);
        OpenPrintCommand = CreateCommand(OpenPrintWindow);
        OpenImportExcelNotificationsCommand = CreateCommand(OpenImportExcelNotificationsWindow);
        OpenExcelImporterCommand = CreateCommand(OpenExcelImporterWindow);
        ApplicationShutdownCommand = CreateCommand(ShutdownApplication);
        OpenImplementedCommand = CreateCommand(OpenImplementedWindow);
        OpenDistributionToTechniciansCommand = CreateCommand(OpenDistributionToTechniciansWindow);

        LogInCommand = new RelayCommand(ExecuteLogIn ,CanExecuteLogIn);
        LogOutCommand = new RelayCommand(ExecuteLogOut ,CanExecuteLogOut);

        GetBasePage();

        // تحديث اسم المستخدم عند تغييره
        CurrentUser.Instance.PropertyChanged += (s ,e) =>
        {
            if(e.PropertyName == nameof(CurrentUser.Username))
            {
                OnPropertyChanged(nameof(CurrentUserName));
            }
        };
    }

    // منطق تسجيل الدخول
    private bool CanExecuteLogIn(object parameter)
    {
        // يمكن تنفيذ تسجيل الدخول إذا لم يكن هناك مستخدم حالي
        return string.IsNullOrEmpty(CurrentUser.Instance.Username);
    }

    private void ExecuteLogIn(object parameter)
    {
        var loginWindow = new LoginWindow();
        loginWindow.ShowDialog();
    }

    // منطق تسجيل الخروج
    private bool CanExecuteLogOut(object parameter)
    {
        return !string.IsNullOrEmpty(CurrentUser.Instance.Username);
    }
   

    private void ExecuteLogOut(object parameter)
    {
        CurrentUser.Instance.ClearUser();
        OnPropertyChanged(nameof(CurrentUserName));
        Summary = "تم تسجيل الخروج بنجاح!";
    }

    // إغلاق التطبيق
    private void ShutdownApplication()
    {
        var result = MessageBox.Show("هل تريد الخروج من البرنامج؟" ,"تنبيه" ,
            MessageBoxButton.YesNo ,MessageBoxImage.Question ,MessageBoxResult.No ,MessageBoxOptions.RtlReading);

        if(result == MessageBoxResult.Yes)
        {
            Application.Current.Shutdown();
        }
    }

    // فتح النوافذ
    private void OpenHomeWindow()
    {
        OpenWindow<HomeWindow>();
    }

    private void OpenAboutWindow()
    {
        OpenWindow<AboutView>();
    }

    private void OpenPrintWindow()
    {
        OpenWindow<PrintWindow>();
    }

    private void OpenDistributionToTechniciansWindow()
    {
        OpenWindow<DistributionToTechniciansWindow>();
    }

    private void OpenImplementedWindow()
    {
        OpenWindow<AddVisitDataWindow>();
    }

    private void OpenImportExcelNotificationsWindow()
    {
        OpenWindow<ImportSAPDataExcelWindow>();
    }

    private void OpenExcelImporterWindow()
    {
        OpenWindow<Excel_ImporterWindow>();
    }

    private void OpenEmployeeWindow()
    {
        OpenWindow<EmployeeWindow>();
    }

    // دالة مساعدة لفتح النوافذ
    private void OpenWindow<T>() where T : UserControl, new()
    {
        var window = Application.Current.Windows.OfType<T>().FirstOrDefault() ?? new T();
        if(_mainWindow != null)
        {
            _mainWindow.MainContentControl.Content = window;
        }
    }

    // الحصول على النافذة الرئيسية
    public static void GetBasePage()
    {
        _mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
    }  
}
