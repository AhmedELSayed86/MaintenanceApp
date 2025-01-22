using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.LinkedTables;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels;

public class AddVisitDataViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;
    public ObservableCollection<SAPData> SAPDataRecords { get; set; } = new();
    public ObservableCollection<VisitData> VisitDataRecords { get; set; } = new();
    public ObservableCollection<LinkedVisitDatasToSAPDatas> LinkedVisitDatasToSAPDatasRecords { get; set; } = new();
    public ObservableCollection<SAPData> FilteredSAPDataRecords { get; set; } = new();
    private readonly EmployeeDataViewModel _employeeDataViewModel;

    public ObservableCollection<EmployeeData> EmployeeRecords { get; set; } = new();
    public ObservableCollection<EmployeeData> FilteredEmployeeRecords { get; set; } = new();
    private EmployeeData _selectedEmployee;
    public EmployeeData SelectedEmployee
    {
        get => _selectedEmployee;
        set
        {
            _selectedEmployee = value;            
            EmployeeName = _selectedEmployee?.Name ?? string.Empty;
            ApplyEmployeeFilter();
            OnPropertyChanged(nameof(SelectedEmployee));
        }
    }

    private string _employeeName;
    public string EmployeeName
    {
        get => _employeeName;
        set
        {
            _employeeName = value;
            ApplyEmployeeFilter();
            OnPropertyChanged(nameof(EmployeeName));
        }
    }

    private string _employeeSearchKeyword;
    public string EmployeeSearchKeyword
    {
        get => _employeeSearchKeyword;
        set
        {
            _employeeSearchKeyword = value;
            ApplyEmployeeFilter();
            OnPropertyChanged(nameof(EmployeeSearchKeyword));
        }
    } 

    private int? _employeeCode;
    public int? EmployeeCode
    {
        get => _employeeCode;
        set
        {
            _employeeCode = value;
            ApplyEmployeeFilter();
            OnPropertyChanged(nameof(EmployeeCode));
        }
    } 

    private SAPData _selectedSAPData;
    public SAPData SelectedSAPData
    {
        get => _selectedSAPData;
        set
        {
            _selectedSAPData = value;
            LoadVisitData();
            OnPropertyChanged(nameof(SelectedSAPData));
        }
    }

    private VisitData _selectedVisitData;
    public VisitData SelectedVisitData
    {
        get => _selectedVisitData;
        set
        {
            _selectedVisitData = value;
            OnPropertyChanged(nameof(SelectedVisitData)); // إعلام واجهة المستخدم بالتغيير
        }
    }

    private string _newNotification;
    public string NewNotification
    {
        get => _newNotification;
        set
        {
            _newNotification = value;
            OnPropertyChanged(nameof(NewNotification));
        }
    }

    private string _searchKeyword;
    public string SearchKeyword
    {
        get => _searchKeyword;
        set
        {
            _searchKeyword = value;
            ApplyFilter();
            ApplyEmployeeFilter();
            OnPropertyChanged(nameof(SearchKeyword));
        }
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddSAPDataCommand { get; }
    public ICommand AddVisitDataCommand { get; }
    public ICommand DeleteVisitDataCommand { get; }
    public ICommand UpdateVisitDataCommand { get; }

    public AddVisitDataViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
        _employeeDataViewModel = new EmployeeDataViewModel(databaseHelper) ?? throw new ArgumentNullException(nameof(EmployeeDataViewModel));

        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
        AddSAPDataCommand = new RelayCommand(async _ => await AddSAPDataAsync());
        AddVisitDataCommand = new RelayCommand(async _ => await AddVisitDataAsync());
        DeleteVisitDataCommand = new RelayCommand(async _ => await DeleteVisitDataAsync());
        UpdateVisitDataCommand = new RelayCommand(async _ => await UpdateVisitDataAsync());

        // تحميل البيانات عند بدء التطبيق
        _ = LoadDataAsync(); 
        LoadEmployeeData();
    }

    private async void LoadEmployeeData()
    {
        try
        {
            // تحميل البيانات من قاعدة البيانات
            var employeeList = await _databaseHelper.GetAllRecordsAsync<EmployeeData>();

            if(employeeList != null && employeeList.Any())
            {
                EmployeeRecords = new ObservableCollection<EmployeeData>(employeeList);
                FilteredEmployeeRecords = new ObservableCollection<EmployeeData>(EmployeeRecords);
                OnPropertyChanged(nameof(FilteredEmployeeRecords));
            }
            else
            {
                MessageBox.Show("لا توجد بيانات موظفين لعرضها.");
            }
        }
        catch(Exception ex)
        {
            MessageBox.Show($"خطأ في تحميل بيانات الموظفين: {ex.Message}");
        }
    }

    private void ApplyEmployeeFilter()
    {
        if(string.IsNullOrWhiteSpace(EmployeeSearchKeyword))
        {
            FilteredEmployeeRecords = new ObservableCollection<EmployeeData>(EmployeeRecords);
        }
        else
        {
            FilteredEmployeeRecords = new ObservableCollection<EmployeeData>(
                EmployeeRecords.Where(e =>
                    e.Name.Contains(EmployeeSearchKeyword)));
        }
        OnPropertyChanged(nameof(FilteredEmployeeRecords));
    }

    private async Task LoadDataAsync()
    {
        try
        {
            SAPDataRecords.Clear();
            var sapDataList = await _databaseHelper.GetAllRecordsAsync<SAPData>();
            foreach(var sap in sapDataList)
            {
                SAPDataRecords.Add(sap);
            }
            FilteredSAPDataRecords = new ObservableCollection<SAPData>(SAPDataRecords);
            OnPropertyChanged(nameof(FilteredSAPDataRecords));
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error loading data: {ex.Message}");
        }
    }


    private void ApplyFilter()
    {
        if(string.IsNullOrWhiteSpace(SearchKeyword))
        {
            FilteredSAPDataRecords = new ObservableCollection<SAPData>(SAPDataRecords);
            MessageBox.Show($"ApplyFilterif FilteredSAPDataRecords.Count: {FilteredSAPDataRecords.Count}");
        }
        else
        {
            FilteredSAPDataRecords = new ObservableCollection<SAPData>(
                SAPDataRecords.Where(s => s.Notification.ToString()?.Contains(SearchKeyword) == true));
            MessageBox.Show($"ApplyFilterelse FilteredSAPDataRecords.Count: {FilteredSAPDataRecords.Count}");
        }
    }


    private async Task AddSAPDataAsync()
    {
        if(string.IsNullOrWhiteSpace(NewNotification)) return;

        try
        {
            var newSAPData = new SAPData
            {
                Notification = Convert.ToDouble(NewNotification) ,
                NotificationType = "New Type" ,
                NotifDate = DateTime.Now
            };

            await _databaseHelper.InsertRecordAsync(newSAPData);
            SAPDataRecords.Add(newSAPData);
            FilteredSAPDataRecords.Add(newSAPData);

            NewNotification = string.Empty; // إعادة تعيين الإدخال
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error adding SAP data: {ex.Message}");
        }
    }

    private async void LoadVisitData()
    {
        try
        {
            VisitDataRecords.Clear();

            var linkedVisitData = await _databaseHelper.GetFilteredLinkedRecordsAsync(
                "VisitDatas" ,    // الجدول الأول
                "SAPDatas" ,      // الجدول الثاني
                "EmployeeDatas" , // الجدول الثالث
                "Notification" ,  // عمود الربط بين VisitDatas و SAPDatas
                SelectedSAPData.Notification.ToString() // قيمة الإشعار المحدد
            );

            if(linkedVisitData == null || linkedVisitData.Count() == 0) return;

            foreach(var data in linkedVisitData)
            {
                LinkedVisitDatasToSAPDatasRecords.Add(new LinkedVisitDatasToSAPDatas
                {
                    ID = data.ID ,
                    Notification = data.Notification ,
                    VisitDate = data.VisitDate ,
                    Technician = data.TechnicianName , // اسم الفني
                    ServiceDetails = data.ServiceDetails ,
                    ListName = data.ListName ,
                    Implemented = data.Implemented ,
                    TechnicianName = data.TechnicianName
                });
            }

            OnPropertyChanged(nameof(VisitDataRecords));
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error loading data: {ex.Message}");
        }
    }

    private async Task AddVisitDataAsync()
    {
        if(SelectedSAPData == null) return;

        try
        {
            var newVisitData = new VisitData
            {
                Notification = SelectedSAPData.Notification ,
                VisitDate = DateTime.Now ,
                Technician = "New Technician" ,
                ServiceDetails = "New Service" ,
                Paid = 0 ,
                Unpaid = 0
            };

            await _databaseHelper.InsertRecordAsync(newVisitData);
            SelectedSAPData.Visits.Add(newVisitData);
            LoadVisitData();
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error adding visit data: {ex.Message}");
        }
    }

    private async Task DeleteVisitDataAsync()
    {
        if(SelectedVisitData == null) return;

        try
        {
            await _databaseHelper.DeleteRecordAsync(SelectedVisitData);
            SelectedSAPData.Visits.Remove(SelectedVisitData);
            LoadVisitData();
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error deleting visit data: {ex.Message}");
        }
    }

    private async Task UpdateVisitDataAsync()
    {
        if(SelectedVisitData == null) return;

        try
        {
            SelectedVisitData.Technician = "Updated Technician";
            await _databaseHelper.UpdateRecordAsync(SelectedVisitData);
            LoadVisitData();
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error updating visit data: {ex.Message}");
        }
    }
}