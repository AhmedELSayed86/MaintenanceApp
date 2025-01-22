using DocumentFormat.OpenXml.Wordprocessing;
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
            if(_selectedEmployee != value) // تحقق من تغيير القيمة
            {
                _selectedEmployee = value;
                EmployeeName = _selectedEmployee?.Name ?? string.Empty;
                OnPropertyChanged(nameof(SelectedEmployee));
            }
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
            if(_searchKeyword != value) // تحقق من تغيير القيمة لتجنب التكرار
            {
                _searchKeyword = value;
                ApplyEmployeeFilter(); // استدعاء التصفية
                OnPropertyChanged(nameof(SearchKeyword));
            }
        }
    }

    private string _technicianCode;
    public string TechnicianCode
    {
        get => _technicianCode;
        set
        {
            _technicianCode = value;
            OnPropertyChanged(nameof(TechnicianCode));
        }
    }

    private int _loggedInUser;
    public int LoggedInUser
    {
        get => _loggedInUser;
        set
        {
            _loggedInUser = value;
            OnPropertyChanged(nameof(LoggedInUser));
        }
    }

    private string _summary;
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

    private bool _isLoading = false;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddSAPDataCommand { get; }
    public ICommand AddTechnicianVisitCommand { get; }
    public ICommand AddVisitDataCommand { get; }
    public ICommand DeleteVisitDataCommand { get; }
    public ICommand UpdateVisitDataCommand { get; }

    public AddVisitDataViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
        _employeeDataViewModel = new EmployeeDataViewModel(databaseHelper) ?? throw new ArgumentNullException(nameof(EmployeeDataViewModel));

        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync()); 
        AddSAPDataCommand = new RelayCommand(async _ => await AddSAPDataAsync());
        AddTechnicianVisitCommand = new RelayCommand(async _ => await AddTechnicianVisitAsync());
        AddVisitDataCommand = new RelayCommand(async _ => await AddVisitDataAsync());
        DeleteVisitDataCommand = new RelayCommand(async _ => await DeleteVisitDataAsync());
        UpdateVisitDataCommand = new RelayCommand(async _ => await UpdateVisitDataAsync());

        // تحميل البيانات عند بدء التطبيق
        _ = LoadDataAsync();
        LoadEmployeeData();
    }

    private async Task AddTechnicianVisitAsync()
    {
        if(string.IsNullOrWhiteSpace(SelectedEmployee.Code.ToString()))
        {
            MessageBox.Show("يرجى إدخال كود الفني.");
            return;
        }

        try
        {
            var newVisit = new VisitData
            {
                 Technician = SelectedEmployee.Code ,
                VisitDate = DateTime.Now ,
                CreatedBy = LoggedInUser
            };

            await _databaseHelper.InsertRecordAsync(newVisit);
            Summary = "تمت التوزيع بنجاح.";
            MessageBox.Show("تمت إضافة الزيارة بنجاح.");
        }
        catch(Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء إضافة الزيارة: {ex.Message}");
        }
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
        if(string.IsNullOrWhiteSpace(SearchKeyword))
        {
            // إذا كانت قائمة التصفية مطابقة للبيانات الأصلية، لا تقم بتغييرها
            if(!FilteredEmployeeRecords.SequenceEqual(EmployeeRecords))
            {
                FilteredEmployeeRecords = new ObservableCollection<EmployeeData>(EmployeeRecords);
                OnPropertyChanged(nameof(FilteredEmployeeRecords));
            }
        }
        else
        {
            var filtered = EmployeeRecords.Where(e =>
                (!string.IsNullOrEmpty(e.Name) && e.Name.Contains(SearchKeyword)) ||
                (!string.IsNullOrEmpty(e.Code.ToString()) && e.Code.ToString().Contains(SearchKeyword))).ToList();

            // إذا كانت قائمة التصفية مطابقة للبيانات الجديدة، لا تقم بتغييرها
            if(!FilteredEmployeeRecords.SequenceEqual(filtered))
            {
                FilteredEmployeeRecords = new ObservableCollection<EmployeeData>(filtered);
                OnPropertyChanged(nameof(FilteredEmployeeRecords));
            }
        }
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
                Technician = SelectedEmployee.Code ,
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