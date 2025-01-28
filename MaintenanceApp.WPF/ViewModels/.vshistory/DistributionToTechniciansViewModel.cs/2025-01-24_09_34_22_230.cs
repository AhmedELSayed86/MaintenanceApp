using Dapper.Contrib.Extensions;
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
    private readonly EmployeeDataViewModel _employeeDataViewModel;

    // قوائم البيانات
    public ObservableCollection<SAPDataWithStatus> SAPDataRecords { get; set; } = new();
    public ObservableCollection<VisitData> VisitDataRecords { get; set; } = new();
    public ObservableCollection<LinkedVisitDatasToSAPDatas> LinkedVisitDatasToSAPDatasRecords { get; set; } = new();
    public ObservableCollection<SAPDataWithStatus> FilteredSAPDataRecords { get; set; } = new();
    public ObservableCollection<EmployeeData> EmployeeRecords { get; set; } = new();
    public ObservableCollection<EmployeeData> FilteredEmployeeRecords { get; set; } = new();

    // الخصائص
    private EmployeeData _selectedEmployee;
    public EmployeeData SelectedEmployee
    {
        get => _selectedEmployee;
        set => SetProperty(ref _selectedEmployee ,value ,onChanged: () =>
        {
            EmployeeName = _selectedEmployee?.Name ?? string.Empty;
            ApplyEmployeeFilter();
        });
    }

    private string _employeeName;
    public string EmployeeName
    {
        get => _employeeName;
        set => SetProperty(ref _employeeName ,value ,onChanged: ApplyEmployeeFilter);
    }

    private int? _employeeCode;
    public int? EmployeeCode
    {
        get => _employeeCode;
        set => SetProperty(ref _employeeCode ,value ,onChanged: ApplyEmployeeFilter);
    }

    private IList _selectedSAPDataRecords;
    public IList SelectedSAPDataRecords
    {
        get => _selectedSAPDataRecords;
        set => SetProperty(ref _selectedSAPDataRecords ,value);
    }

    private SAPData _selectedSAPData;
    public SAPData SelectedSAPData
    {
        get => _selectedSAPData;
        set => SetProperty(ref _selectedSAPData ,value ,onChanged: LoadVisitData);
    }

    private VisitData _selectedVisitData;
    public VisitData SelectedVisitData
    {
        get => _selectedVisitData;
        set => SetProperty(ref _selectedVisitData ,value);
    }

    private string _newNotification;
    public string NewNotification
    {
        get => _newNotification;
        set => SetProperty(ref _newNotification ,value);
    }

    private DateTime _visitDate = DateTime.Today;
    public DateTime VisitDate
    {
        get => _visitDate;
        set => SetProperty(ref _visitDate ,value);
    }

    private string _searchKeyword;
    public string SearchKeyword
    {
        get => _searchKeyword;
        set => SetProperty(ref _searchKeyword ,value ,onChanged: () =>
        {
            ApplyEmployeeFilter();
            var employee = EmployeeRecords.FirstOrDefault(e => e.Code.ToString() == _searchKeyword);
            if(employee != null)
            {
                SelectedEmployee = employee;
                EmployeeName = employee.Name;
            }
        });
    }

    private string _technicianCode;
    public string TechnicianCode
    {
        get => _technicianCode;
        set => SetProperty(ref _technicianCode ,value);
    }

    private int _loggedInUser;
    public int LoggedInUser
    {
        get => _loggedInUser;
        set => SetProperty(ref _loggedInUser ,value);
    }

    private string _summary;
    public string Summary
    {
        get => _summary;
        set => SetProperty(ref _summary ,value ,onChanged: () =>
        {
            _ = MessageController.SummaryAsync(Summary).ConfigureAwait(false);
            CommandManager.InvalidateRequerySuggested();
        });
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading ,value ,onChanged: () => CommandManager.InvalidateRequerySuggested());
    }

    private Brush _technicianTextBoxBackground = Brushes.Black;
    public Brush TechnicianTextBoxBackground
    {
        get => _technicianTextBoxBackground;
        set => SetProperty(ref _technicianTextBoxBackground ,value);
    }

    private Brush _sAPDataBackground = Brushes.LawnGreen;
    public Brush SAPDataBackground
    {
        get => _sAPDataBackground;
        set => SetProperty(ref _sAPDataBackground ,value);
    }

    private Brush _visitDateBackground = Brushes.Black;
    public Brush VisitDateBackground
    {
        get => _visitDateBackground;
        set => SetProperty(ref _visitDateBackground ,value);
    }

    private DateTime _myTime = DateTime.Today.AddDays(-6);
    public DateTime MyTime
    {
        get => _myTime;
        set => SetProperty(ref _myTime ,value);
    }

    // الأوامر
    public ICommand LoadDataCommand { get; }
    public ICommand AddVisitDataCommand { get; }
    public ICommand DeleteVisitDataCommand { get; }
    public ICommand UpdateVisitDataCommand { get; }
    public ICommand DistributeCommand { get; }


    public DistributionToTechniciansViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
        _employeeDataViewModel = new EmployeeDataViewModel(databaseHelper) ?? throw new ArgumentNullException(nameof(EmployeeDataViewModel));

        // تهيئة الأوامر
        LoadDataCommand = CreateCommand(async () => await LoadDataAsync());
        AddVisitDataCommand = CreateCommand(async () => await AddVisitDataAsync());
        DeleteVisitDataCommand = CreateCommand(async () => await DeleteVisitDataAsync());
        UpdateVisitDataCommand = CreateCommand(async () => await UpdateVisitDataAsync());
        DistributeCommand = CreateCommand(async () => await DistributionToTechniciansAsync());

        // تحميل البيانات عند بدء التطبيق
        _ = LoadDataAsync();
        LoadEmployeeData();
    }


    private void ResetErrorColor()
    {
        TechnicianTextBoxBackground = Brushes.Black;
        SAPDataBackground = Brushes.LawnGreen;
        VisitDateBackground = Brushes.Black;
    }

    private async Task DistributionToTechniciansAsync()
    {
        ResetErrorColor();
        bool isValid = true;

        if(SelectedEmployee == null)
        {
            TechnicianTextBoxBackground = Brushes.IndianRed;
            await MessageController.SummaryAsync("يجب إدخال كود الفني.").ConfigureAwait(false);
            isValid = false;
        }

        if(VisitDate == default)
        {
            VisitDateBackground = Brushes.IndianRed;
            await MessageController.SummaryAsync("يجب تحديد تاريخ الزيارة").ConfigureAwait(false);
            isValid = false;
        }

        if(SelectedSAPDataRecords == null || SelectedSAPDataRecords.Count == 0)
        {
            SAPDataBackground = Brushes.IndianRed;
            await MessageController.SummaryAsync("يجب تحديد بلاغ.").ConfigureAwait(false);
            isValid = false;
        }

        if(!isValid) return;

        try
        {
            await _databaseHelper.ExecuteInTransactionAsync(async (connection ,transaction) =>
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

                        await connection.InsertAsync(newVisit ,transaction);

                        var existingSAPData = await connection.GetAsync<SAPData>(sapData.Notification ,transaction);
                        if(existingSAPData != null)
                        {
                            existingSAPData.StatusID = 1;
                            await connection.UpdateAsync(existingSAPData ,transaction);
                        }
                        else
                        {
                            throw new Exception($"لم يتم العثور على السجل ذو الإشعار {sapData.Notification} في جدول SAPData.");
                        }
                    }
                }
            });

            await LoadDataAsync();
            LoadVisitData();
            await MessageController.SummaryAsync("تم التوزيع بنجاح.").ConfigureAwait(false);
        }
        catch(Exception ex)
        {
            LogError("حدث خطأ أثناء التوزيع" ,ex);
            await MessageController.SummaryAsync($"حدث خطأ أثناء التوزيع: {ex.Message}").ConfigureAwait(false);
        }
    }

    private async void LoadEmployeeData()
    {
        try
        {
            var employeeList = await _databaseHelper.GetAllRecordsAsync<EmployeeData>();
            if(employeeList != null && employeeList.Any())
            {
                EmployeeRecords = new ObservableCollection<EmployeeData>(employeeList);
                FilteredEmployeeRecords = new ObservableCollection<EmployeeData>(EmployeeRecords);
                OnPropertyChanged(nameof(FilteredEmployeeRecords));
            }
            else
            {
                await MessageController.SummaryAsync("لا توجد بيانات موظفين لعرضها.").ConfigureAwait(false);
            }
        }
        catch(Exception ex)
        {
            LogError("خطأ في تحميل بيانات الموظفين" ,ex);
            await MessageController.SummaryAsync($"خطأ في تحميل بيانات الموظفين: {ex.Message}").ConfigureAwait(false);
        }
    }

    private void ApplyEmployeeFilter()
    {
        if(string.IsNullOrWhiteSpace(SearchKeyword))
        {
            FilteredEmployeeRecords = new ObservableCollection<EmployeeData>(EmployeeRecords);
        }
        else
        {
            var filtered = EmployeeRecords.Where(e =>
                (!string.IsNullOrEmpty(e.Name) && e.Name.Contains(SearchKeyword)) ||
                (!string.IsNullOrEmpty(e.Code.ToString()) && e.Code.ToString().Contains(SearchKeyword))).ToList();

            FilteredEmployeeRecords = new ObservableCollection<EmployeeData>(filtered);
        }
        OnPropertyChanged(nameof(FilteredEmployeeRecords));
    }

    private async Task LoadDataAsync()
    {
        try
        {
            SAPDataRecords.Clear();
            var sapDataList = await _databaseHelper.GetFilteredSAPData0Async();
            foreach(var sap in sapDataList)
            {
                SAPDataRecords.Add(sap);
            }
            FilteredSAPDataRecords = new ObservableCollection<SAPDataWithStatus>(SAPDataRecords);
            OnPropertyChanged(nameof(FilteredSAPDataRecords));
        }
        catch(Exception ex)
        {
            LogError("Error loading data" ,ex);
            await MessageController.SummaryAsync($"Error loading data: {ex.Message}").ConfigureAwait(false);
        }
    }

    private async void LoadVisitData()
    {
        try
        {
            VisitDataRecords.Clear();
            var linkedVisitData = await _databaseHelper.GetFilteredLinkedDateRecordsAsync(
                nameof(VisitData) ,
                nameof(SAPData) ,
                nameof(EmployeeData) ,
                nameof(VisitData.Notification) ,
                SelectedSAPData.Notification.ToString() ,
                DateTime.Today);

            if(linkedVisitData == null || !linkedVisitData.Any()) return;

            foreach(var data in linkedVisitData)
            {
                LinkedVisitDatasToSAPDatasRecords.Add(new LinkedVisitDatasToSAPDatas
                {
                    ID = data.ID ,
                    Notification = data.Notification ,
                    VisitDate = data.VisitDate ,
                    Technician = data.Technician ,
                    ServiceDetails = data.ServiceDetails ,
                    ListName = data.ListName ,
                    Implemented = data.Implemented ,
                    TechnicianName = data.TechnicianName
                });
            }

            OnPropertyChanged(nameof(LinkedVisitDatasToSAPDatasRecords));
        }
        catch(Exception ex)
        {
            LogError("Error loading visit data" ,ex);
            await MessageController.SummaryAsync($"Error loading data: {ex.Message}").ConfigureAwait(false);
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
            LogError("Error adding visit data" ,ex);
            await MessageController.SummaryAsync($"Error adding visit data: {ex.Message}").ConfigureAwait(false);
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
            LogError("Error deleting visit data" ,ex);
            await MessageController.SummaryAsync($"Error deleting visit data: {ex.Message}").ConfigureAwait(false);
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
            LogError("Error updating visit data" ,ex);
            await MessageController.SummaryAsync($"Error updating visit data: {ex.Message}").ConfigureAwait(false);
        }
    }

    public void myTime()
    {
        MyTime = DateTime.Today.AddDays(-6);
    }

      

 

    private void ApplyFilter()
    {
        if(string.IsNullOrWhiteSpace(SearchKeyword))
        {
            FilteredSAPDataRecords = new ObservableCollection<SAPDataWithStatus>(SAPDataRecords);
            _ = MessageController.SummaryAsync($"ApplyFilterif FilteredSAPDataRecords.Count: {FilteredSAPDataRecords.Count}").ConfigureAwait(false);
        }
        else
        {
            FilteredSAPDataRecords = new ObservableCollection<SAPDataWithStatus>(
                SAPDataRecords.Where(s => s.Notification.ToString()?.Contains(SearchKeyword) == true));
            _ = MessageController.SummaryAsync($"ApplyFilterelse FilteredSAPDataRecords.Count: {FilteredSAPDataRecords.Count}").ConfigureAwait(false);
        }
    } 
}
