using MaintenanceApp.WPF.Controllers;
using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.LinkedTables;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using MaintenanceApp.WPF.Controllers;
using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.LinkedTables;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace MaintenanceApp.WPF.ViewModels;

public class DistributionToTechniciansViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;
    public ObservableCollection<SAPDataWithStatus> SAPDataRecords { get; set; } = new();
    public ObservableCollection<VisitData> VisitDataRecords { get; set; } = new();
    public ObservableCollection<LinkedVisitDatasToSAPDatas> LinkedVisitDatasToSAPDatasRecords { get; set; } = new();
    public ObservableCollection<SAPDataWithStatus> FilteredSAPDataRecords { get; set; } = new();
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

    private IList _selectedSAPDataRecords;
    public IList SelectedSAPDataRecords
    {
        get => _selectedSAPDataRecords;
        set
        {
            _selectedSAPDataRecords = value;
            OnPropertyChanged(nameof(SelectedSAPDataRecords));
        }
    }

    private SAPData _selectedSAPData;
    public SAPData SelectedSAPData
    {
        get => _selectedSAPData;
        set
        {
            _selectedSAPData = value;
            //  LoadVisitData();
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

    private DateTime _visitDate;
    public DateTime VisitDate
    {
        get => _visitDate;
        set
        {
            _visitDate = value;
            OnPropertyChanged(nameof(VisitDate));
        }
    }

    private string _searchKeyword;
    public string SearchKeyword
    {
        get => _searchKeyword;
        set
        {
            if(_searchKeyword != value)
            {
                _searchKeyword = value;
                ApplyEmployeeFilter(); // تطبيق التصفية عند تغيير الكلمة المفتاحية

                // البحث عن الموظف بناءً على الكود المكتوب
                var employee = EmployeeRecords.FirstOrDefault(e => e.Code.ToString() == _searchKeyword);
                if(employee != null)
                {
                    SelectedEmployee = employee; // تحديث الموظف المحدد
                    EmployeeName = employee.Name; // تحديث اسم الموظف
                }

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
            MessageController.SummaryAsync(Summary);
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

    private Brush _technicianTextBoxBackground = Brushes.Black;
    public Brush TechnicianTextBoxBackground
    {
        get => _technicianTextBoxBackground;
        set
        {
            _technicianTextBoxBackground = value;
            OnPropertyChanged(nameof(TechnicianTextBoxBackground));
        }
    }

    private Brush _sAPDataBackground = Brushes.LawnGreen;
    public Brush SAPDataBackground
    {
        get => _sAPDataBackground;
        set
        {
            _sAPDataBackground = value;
            OnPropertyChanged(nameof(SAPDataBackground));
        }
    }

    private Brush _visitDateBackground = Brushes.Black;
    public Brush VisitDateBackground
    {
        get => _visitDateBackground;
        set
        {
            _visitDateBackground = value;
            OnPropertyChanged(nameof(VisitDateBackground));
        }
    }

    private DateTime _myTime = DateTime.Today.AddDays(-6);
    public DateTime MyTime
    {
        get => _myTime;
        set
        {
            _myTime = value;
            OnPropertyChanged(nameof(VisitDateBackground));
        }
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddSAPDataCommand { get; }
    public ICommand AddVisitDataCommand { get; }
    public ICommand DeleteVisitDataCommand { get; }
    public ICommand UpdateVisitDataCommand { get; }
    public ICommand DistributeCommand { get; }

    public DistributionToTechniciansViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
        _employeeDataViewModel = new EmployeeDataViewModel(databaseHelper) ?? throw new ArgumentNullException(nameof(EmployeeDataViewModel));

        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
        AddSAPDataCommand = new RelayCommand(async _ => await AddSAPDataAsync());
        AddVisitDataCommand = new RelayCommand(async _ => await AddVisitDataAsync());
        DeleteVisitDataCommand = new RelayCommand(async _ => await DeleteVisitDataAsync());
        UpdateVisitDataCommand = new RelayCommand(async _ => await UpdateVisitDataAsync());

        // الأمر الجديد للتوزيع
        DistributeCommand = new RelayCommand(async _ => await DistributionToTechniciansAsync());

        // تحميل البيانات عند بدء التطبيق
        _ = LoadDataAsync();
        LoadEmployeeData();

        VisitDate = DateTime.Today; // تعيين تاريخ اليوم كقيمة افتراضية
    }

    private void ResetErrorColor()
    {
        TechnicianTextBoxBackground = Brushes.Black;
        SAPDataBackground = Brushes.LawnGreen;
        VisitDateBackground = Brushes.Black;
    }

    private async Task DistributionToTechniciansAsync()
    {
        bool chek = true;
        ResetErrorColor();
        if(SelectedEmployee == null)
        {
            TechnicianTextBoxBackground = Brushes.IndianRed;
            MessageController.SummaryAsync("يجب إدخال كود الفني.");
            chek = false;
        }

        if(VisitDate == null || VisitDate.ToString() == "0001-01-01 12:00:00 AM")
        {
            VisitDateBackground = Brushes.IndianRed;
            MessageController.SummaryAsync("يجب تحديد تاريخ الزيارة");
            chek = false;
        }

        if(SelectedSAPDataRecords == null || SelectedSAPDataRecords.Count == 0)
        {
            SAPDataBackground = Brushes.IndianRed;
            MessageController.SummaryAsync("يجب تحديد بلاغ.");
            chek = false;
        }

        if(chek == false)
        {
            return;
        }
        try
        {
            foreach(var selectedItem in SelectedSAPDataRecords)
            {
                if(selectedItem is SAPDataWithStatus sapData)
                {
                    var newVisit = new VisitData
                    {
                        Notification = sapData.Notification ,
                        VisitDate = VisitDate ,
                        Technician = SelectedEmployee.Code ,
                        Notes = string.Empty ,
                        UserStatus = string.Empty ,
                        CreatedBy = LoggedInUser ,
                        CreatedOn = DateTime.UtcNow
                    };

                    await _databaseHelper.InsertRecordAsync(newVisit);
                }
            }

            MessageController.SummaryAsync("تمت التوزيع بنجاح.");
        }
        catch(Exception ex)
        {
            MessageController.SummaryAsync($"حدث خطأ أثناء التوزيع: {ex.Message}");
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
                MessageController.SummaryAsync("لا توجد بيانات موظفين لعرضها.");
            }
        }
        catch(Exception ex)
        {
            MessageController.SummaryAsync($"خطأ في تحميل بيانات الموظفين: {ex.Message}");
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
            var sapDataList = await _databaseHelper.GetFilteredSAPDataAsync();
            foreach(var sap in sapDataList)
            {
                SAPDataRecords.Add(sap);
            }
            FilteredSAPDataRecords = new ObservableCollection<SAPDataWithStatus>(SAPDataRecords);
            OnPropertyChanged(nameof(FilteredSAPDataRecords));
        }
        catch(Exception ex)
        {
            MessageController.SummaryAsync($"Error loading data: {ex.Message}");
        }
    }


    private void ApplyFilter()
    {
        if(string.IsNullOrWhiteSpace(SearchKeyword))
        {
            FilteredSAPDataRecords = new ObservableCollection<SAPDataWithStatus>(SAPDataRecords);
            MessageController.SummaryAsync($"ApplyFilterif FilteredSAPDataRecords.Count: {FilteredSAPDataRecords.Count}");
        }
        else
        {
            FilteredSAPDataRecords = new ObservableCollection<SAPDataWithStatus>(
                SAPDataRecords.Where(s => s.Notification.ToString()?.Contains(SearchKeyword) == true));
            MessageController.SummaryAsync($"ApplyFilterelse FilteredSAPDataRecords.Count: {FilteredSAPDataRecords.Count}");
        }
    }

    private async Task AddSAPDataAsync()
    {
        if(string.IsNullOrWhiteSpace(NewNotification)) return;

        try
        {
            var newSAPData = new SAPDataWithStatus
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
            MessageController.SummaryAsync($"Error adding SAP data: {ex.Message}");
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
                    Technician = data.Technician , // اسم الفني
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
            MessageController.SummaryAsync($"Error loading data: {ex.Message}");
        }
    }

    private async Task AddVisitDataAsync()
    {
        if(SelectedSAPData == null) return; else MessageController.SummaryAsync("Not Select");

        try
        {
            var newVisitData = new VisitData
            {
                Notification = SelectedSAPData.Notification ,
                VisitDate = SelectedVisitData.VisitDate ,
                Technician = SelectedEmployee.Code ,
                ServiceDetails = SelectedVisitData.ServiceDetails ,
                Implemented = SelectedVisitData.Implemented ,
                Cost = SelectedVisitData.Cost ,
                Paid = SelectedVisitData.Paid ,
                Unpaid = SelectedVisitData.Unpaid ,
                DiscountPercentage = SelectedVisitData.DiscountPercentage ,
                PaymentRefused = SelectedVisitData.PaymentRefused ,
                DistributionStatus = SelectedVisitData.DistributionStatus ,
                Notes = SelectedVisitData.Notes ,
                AssistantTechnician = SelectedVisitData.AssistantTechnician ,
                DeterminationTechnician = SelectedVisitData.DeterminationTechnician ,
                ExecutionDuration = SelectedVisitData.ExecutionDuration ,
                WarrantyStatus = SelectedVisitData.WarrantyStatus ,
                UserStatus = SelectedVisitData.UserStatus ,
                CreatedBy = LoggedInUser ,
                CreatedOn = DateTime.UtcNow
            };

            await _databaseHelper.InsertRecordAsync(newVisitData);
            SelectedSAPData.Visits.Add(newVisitData);
            LoadVisitData();
        }
        catch(Exception ex)
        {
            MessageController.SummaryAsync($"Error adding visit data: {ex.Message}");
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
            MessageController.SummaryAsync($"Error deleting visit data: {ex.Message}");
        }
    }

    private async Task UpdateVisitDataAsync()
    {
        if(SelectedVisitData == null) return;

        try
        {
            SelectedVisitData.Technician = 1;
            await _databaseHelper.UpdateRecordAsync(SelectedVisitData);
            LoadVisitData();
        }
        catch(Exception ex)
        {
            MessageController.SummaryAsync($"Error updating visit data: {ex.Message}");
        }
    }

    public void myTime()
    {
        MyTime = DateTime.Today.AddDays(-6);
    }
}
