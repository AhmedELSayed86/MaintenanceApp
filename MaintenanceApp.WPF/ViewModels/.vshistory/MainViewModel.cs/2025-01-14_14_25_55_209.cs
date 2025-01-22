﻿using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Views;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MaintenanceApp.WPF.ViewModels;

public class MainViewModel : BaseViewModel
{
    private static MainWindow mainWindow;
    public string CurrentUserName => CurrentUser.Instance.Username;

    private Brush _exitBackground;
    public Brush EXITBackground
    {
        get => _exitBackground;
        set
        {
            _exitBackground = value;
            OnPropertyChanged(nameof(EXITBackground));
            // RowSelectedCommand?.Execute(null); // استدعاء الأمر يدويًا عند التغيير
        }
    }

    private string _summary = "بسم الله";
    public string Summary
    {
        get => _summary;
        set
        {
            _summary = value;
            OnPropertyChanged(nameof(Summary));
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    // Commands
    public ICommand LogInCommand { get; }
    public ICommand LogOutCommand { get; }
    public ICommand EXITBackgroundCommand { get; }
    public ICommand OpenHomeCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand PrintCommand { get; }
    public ICommand OpeneEmployeeCommand { get; }
    public ICommand OpenPrintCommand { get; }
    public ICommand OpeneImplementedCommand { get; }
    public ICommand OpeneImportExcelNotificationsCommand { get; }
    public ICommand OpeneExcel_ImporterCommand { get; }
    public ICommand ApplicationShutdownCommand { get; }

    public MainViewModel()
    {
        _exitBackground = Brushes.LightGray;
        OpenHomeCommand = new RelayCommand(o => OpenHomeWindow());
        OpeneEmployeeCommand = new RelayCommand(o => OpeneEmployeeWindow());
        OpenPrintCommand = new RelayCommand(o => OpenPrintWindow());
        OpeneImportExcelNotificationsCommand = new RelayCommand(o => OpeneImportExcelNotificationsWindow());
        OpeneExcel_ImporterCommand = new RelayCommand(o => OpeneExcel_ImporterWindow());
        ApplicationShutdownCommand = new RelayCommand(o => ShutdownApplication());
        OpeneImplementedCommand = new RelayCommand(o => OpeneImplementedWindow());
        // EXITBackgroundCommand = new RelayCommand(o => EXITBackgroundButton());

        // أوامر تسجيل الدخول وتسجيل الخروج
        LogInCommand = new RelayCommand(ExecuteLogIn ,CanExecuteLogIn);
        LogOutCommand = new RelayCommand(ExecuteLogOut ,CanExecuteLogOut); GetBasePage();
          
        CurrentUser.Instance.PropertyChanged += (s ,e) =>
        {
            if(e.PropertyName == nameof(CurrentUser.Username))
            {
                OnPropertyChanged(nameof(CurrentUserName));
            }
        }; 
    }
  
    private bool CanExecuteLogIn(object parameter)
    {
        // يمكن تنفيذ تسجيل الدخول إذا لم يكن هناك مستخدم حالي
        return string.IsNullOrEmpty(CurrentUser.Instance.Username);
    }

    private void ExecuteLogIn(object parameter)
    {
        // نافذة تسجيل الدخول
        var loginWindow = new LoginWindow();
        loginWindow.ShowDialog();
    }

    private bool CanExecuteLogOut(object parameter)
    {
        // يمكن تنفيذ تسجيل الخروج إذا كان هناك مستخدم حالي

        return !string.IsNullOrEmpty(CurrentUser.Instance.Username);
    }

    private void ExecuteLogOut(object parameter)
    {
        CurrentUser.Instance.ClearUser(); // إزالة بيانات المستخدم الحالي
        OnPropertyChanged(nameof(CurrentUserName)); // تحديث اسم المستخدم
       Summary ="Logged out successfully!" ;
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

    private void OpeneImplementedWindow()
    {
        AddVisitDataWindow addDataWindow = System.Windows.Application.Current.Windows.OfType<AddVisitDataWindow>().FirstOrDefault() ?? new AddVisitDataWindow();
        mainWindow.MainContentControl.Content = addDataWindow;
    }

    private void OpeneImportExcelNotificationsWindow()
    {
        ImportSAPDataExcelWindow excelWindow = System.Windows.Application.Current.Windows.OfType<ImportSAPDataExcelWindow>().FirstOrDefault() ?? new ImportSAPDataExcelWindow();

        mainWindow.MainContentControl.Content = excelWindow;
    }

    private void OpeneExcel_ImporterWindow()
    {
        Excel_ImporterWindow excelWindow = System.Windows.Application.Current.Windows.OfType<Excel_ImporterWindow>().FirstOrDefault() ?? new Excel_ImporterWindow();

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
