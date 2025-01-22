using MaintenanceApp.WPF.Controllers;
using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.LinkedTables;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MaintenanceApp.WPF.ViewModels;

public class AddVisitDataViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;
    public ObservableCollection<SAPDataWithStatus> SAPDataRecords { get; set; } = new();
    public ObservableCollection<VisitData> VisitDataRecords { get; set; } = new();
    public ObservableCollection<LinkedVisitDatasToSAPDatas> LinkedVisitDatasToSAPDatasRecords { get; set; } = new();
    public ObservableCollection<SAPDataWithStatus> FilteredSAPDataRecords { get; set; } = new();
    private readonly StatusDataViewModel _StatusDataViewModel;

    public ObservableCollection<StatusData> StatusRecords { get; set; } = new();
    public ObservableCollection<StatusData> FilteredStatusRecords { get; set; } = new();


    private StatusData _selectedStatus;
    public StatusData SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            if(_selectedStatus != value) // تحقق من تغيير القيمة
            {
                _selectedStatus = value;
                StatusName = _selectedStatus?.Name ?? string.Empty;
                OnPropertyChanged(nameof(SelectedStatus));
            }
        }
    }

    private string _StatusName;
    public string StatusName
    {
        get => _StatusName;
        set
        {
            _StatusName = value;
            ApplyStatusFilter();
            OnPropertyChanged(nameof(StatusName));
        }
    }

    private int? _StatusCode;
    public int? StatusCode
    {
        get => _StatusCode;
        set
        {
            _StatusCode = value;
            ApplyStatusFilter();
            OnPropertyChanged(nameof(StatusCode));
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
                ApplyStatusFilter(); // تطبيق التصفية عند تغيير الكلمة المفتاحية

                // البحث عن الموظف بناءً على الكود المكتوب
                var Status = StatusRecords.FirstOrDefault(e => e.Code.ToString() == _searchKeyword);
                if(Status != null)
                {
                    SelectedStatus = Status; // تحديث الموظف المحدد
                    StatusName = Status.Name; // تحديث اسم الموظف
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

    private DateTime _myTime =DateTime.Today.AddDays(-6);
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
    public ICommand AddTechnicianVisitCommand { get; }
    public ICommand AddVisitDataCommand { get; }
    public ICommand DeleteVisitDataCommand { get; }
    public ICommand UpdateVisitDataCommand { get; }

    public AddVisitDataViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
        _StatusDataViewModel = new StatusDataViewModel(databaseHelper) ?? throw new ArgumentNullException(nameof(StatusDataViewModel));

        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
        AddSAPDataCommand = new RelayCommand(async _ => await AddSAPDataAsync());
        AddTechnicianVisitCommand = new RelayCommand(async _ => await AddTechnicianVisitAsync());
        AddVisitDataCommand = new RelayCommand(async _ => await AddVisitDataAsync());
        DeleteVisitDataCommand = new RelayCommand(async _ => await DeleteVisitDataAsync());
        UpdateVisitDataCommand = new RelayCommand(async _ => await UpdateVisitDataAsync());

        // تحميل البيانات عند بدء التطبيق
        _ = LoadDataAsync();
        LoadStatusData();

        VisitDate = DateTime.Today; // تعيين تاريخ اليوم كقيمة افتراضية
    }

    private void ResetErrorColor()
    {
        TechnicianTextBoxBackground = Brushes.Black;
        SAPDataBackground = Brushes.LawnGreen;
        VisitDateBackground = Brushes.Black;
    }

    private async Task AddTechnicianVisitAsync()
    {
        bool chek = true;
        ResetErrorColor();
        if(SelectedStatus == null)
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

        if(SelectedSAPData == null)
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
            var newVisit = new VisitData
            {
                Notification = SelectedSAPData?.Notification ?? throw new InvalidOperationException("SelectedSAPData is null.") ,
                VisitDate = VisitDate ,
                Technician = SelectedStatus?.Code ?? throw new InvalidOperationException("SelectedStatus is null.") ,
                Notes = SelectedVisitData?.Notes ?? string.Empty ,
                UserStatus = SelectedVisitData?.UserStatus ?? string.Empty ,
                CreatedBy = LoggedInUser ,
                CreatedOn = DateTime.UtcNow
            };

            await _databaseHelper.InsertRecordAsync(newVisit);
            Summary = "تمت التوزيع بنجاح.";
            MessageController.SummaryAsync("تمت إضافة الزيارة بنجاح.");
        }
        catch(Exception ex)
        {
            MessageController.SummaryAsync($"حدث خطأ أثناء إضافة الزيارة: {ex.Message}");
        }
    }

    private async void LoadStatusData()
    {
        try
        {
            // تحميل البيانات من قاعدة البيانات
            var StatusList = await _databaseHelper.GetAllRecordsAsync<StatusData>();

            if(StatusList != null && StatusList.Any())
            {
                StatusRecords = new ObservableCollection<StatusData>(StatusList);
                FilteredStatusRecords = new ObservableCollection<StatusData>(StatusRecords);
                OnPropertyChanged(nameof(FilteredStatusRecords));
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

    private void ApplyStatusFilter()
    {
        if(string.IsNullOrWhiteSpace(SearchKeyword))
        {
            // إذا كانت قائمة التصفية مطابقة للبيانات الأصلية، لا تقم بتغييرها
            if(!FilteredStatusRecords.SequenceEqual(StatusRecords))
            {
                FilteredStatusRecords = new ObservableCollection<StatusData>(StatusRecords);
                OnPropertyChanged(nameof(FilteredStatusRecords));
            }
        }
        else
        {
            var filtered = StatusRecords.Where(e =>
                (!string.IsNullOrEmpty(e.Name) && e.Name.Contains(SearchKeyword)) ||
                (!string.IsNullOrEmpty(e.Code.ToString()) && e.Code.ToString().Contains(SearchKeyword))).ToList();

            // إذا كانت قائمة التصفية مطابقة للبيانات الجديدة، لا تقم بتغييرها
            if(!FilteredStatusRecords.SequenceEqual(filtered))
            {
                FilteredStatusRecords = new ObservableCollection<StatusData>(filtered);
                OnPropertyChanged(nameof(FilteredStatusRecords));
            }
        }
    }

    private async Task LoadDataAsync()
    {
        try
        {
            SAPDataRecords.Clear();
            var sapDataList = await _databaseHelper.GetSAPDataWithStatusAsync();
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
                "StatusDatas" , // الجدول الثالث
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
                Technician = SelectedStatus.Code ,
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