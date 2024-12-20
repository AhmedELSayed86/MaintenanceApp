using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using MaintenanceApp.WPF.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private static MainWindow mainWindow;
    public ObservableCollection<VisitData> Records { get; set; } = new ObservableCollection<VisitData>();

    private VisitData _newRecord = new();
    public VisitData NewRecord
    {
        get => _newRecord;
        set
        {
            _newRecord = value;
            OnPropertyChanged();
        }
    }

    public ICommand ApplicationShutdownCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand OpeneImplementedCommand { get; }
    public ICommand OpeneEmployeeCommand { get; }
    public ICommand OpeneImportExcelNotificationsCommand { get; }
    public ICommand OpenPrintWindowCommand { get; }

    public MainViewModel()
    {
        ApplicationShutdownCommand = new RelayCommand(_ => ApplicationShutdown());
        CancelCommand = new RelayCommand(_ => CancelOperation());
        OpeneImplementedCommand = new RelayCommand(_ => OpeneImplementedWindow());
        OpeneEmployeeCommand = new RelayCommand(_ => OpeneEmployeeWindow());
        OpeneImportExcelNotificationsCommand = new RelayCommand(_ => OpeneImportExcelNotificationsWindow());
        GetBasePage(); OpenPrintWindowCommand = new RelayCommand(_ => OpenPrintWindow());
        DatabaseHelper databaseHelper = new DatabaseHelper();
    }

    private void OpenPrintWindow()
    {
        PrintWindow printWindow = null;
        printWindow = new PrintWindow
        {
            DataContext = new PrintViewModel(() => printWindow?.Close())
        };
        printWindow.ShowDialog();
    }

    private void ApplicationShutdown()
    {
        var r = MessageBox.Show("هل تريد الخروج من البرنامج؟" ,"تنبيه" ,MessageBoxButton.YesNo ,MessageBoxImage.Question ,MessageBoxResult.No ,MessageBoxOptions.RtlReading);

        if(r == MessageBoxResult.Yes)
        {
            Application.Current.Shutdown();
        }
    }

    private void CancelOperation()
    {
        NewRecord = new VisitData();
    }

    private void OpeneImplementedWindow()
    {
        AddDataWindow addDataWindow = new();
        mainWindow.MainContentControl.Content = addDataWindow;
    }

    private void OpeneImportExcelNotificationsWindow()
    {
        ImportExcelWindow addDataWindow = new();
        addDataWindow.Show();
    }

    private void OpeneEmployeeWindow()
    {
        EmployeeWindow employeeWindow = new();
        employeeWindow.Show();
    }


    public static void GetBasePage()
    {
        //mainWindow = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this ,new PropertyChangedEventArgs(propertyName));
}
