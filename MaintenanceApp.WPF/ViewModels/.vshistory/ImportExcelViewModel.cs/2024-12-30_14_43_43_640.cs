using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels;

public class ImportExcelViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;

    private DataTable _excelData;
    public DataTable ExcelData
    {
        get => _excelData;
        set
        {
            _excelData = value; UpdateFilteredData();
            OnPropertyChanged(nameof(ExcelData));
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    private ObservableCollection<SAPData> _excelDataList;
    public ObservableCollection<SAPData> ExcelDataList
    {
        get => _excelDataList;
        set
        {
            _excelDataList = value; UpdateFilteredData();
            OnPropertyChanged(nameof(ExcelDataList));
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    private ObservableCollection<SAPData> _filteredExcelDataList;
    public ObservableCollection<SAPData> FilteredExcelDataList
    {
        get => _filteredExcelDataList;
        set
        {
            _filteredExcelDataList = value;
            OnPropertyChanged(nameof(FilteredExcelDataList));
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    private string _searchQuery;
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            OnPropertyChanged(nameof(SearchQuery));
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
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

    public ICommand LoadExcelCommand { get; }
    public ICommand SaveSAPDataToDatabaseCommand { get; }
    public ICommand SaveEmployeeToDatabaseCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand SearchCommand { get; }

    public ImportExcelViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
        LoadExcelCommand = new RelayCommand(async o => await LoadExcelAsync());
        SaveSAPDataToDatabaseCommand = new RelayCommand(async o => await SaveSAPDataToDatabaseAsync() ,o => CanSaveToDatabase());
        SaveEmployeeToDatabaseCommand = new RelayCommand(async o => await SaveEmployeeDataToDatabaseAsync() ,o => CanSaveToDatabase());
        CloseCommand = new RelayCommand(o => GotoHome());
        SearchCommand = new RelayCommand(o => ExecuteSearch());
    }

    private void GotoHome()
    {
        MainViewModel mainViewModel = System.Windows.Application.Current.Windows.OfType<MainViewModel>().FirstOrDefault() ?? new MainViewModel();
        mainViewModel.OpenHomeCommand.Execute(this);
    }

    private void CheckData()
    {
        FilteredExcelDataList = new ObservableCollection<SAPData>(ExcelDataList
            .Where(item => item.NotificationType != null && item.NotificationType.Contains("بحثك هنا")));
    }

    private void ExecuteSearch()
    {
        // منطق البحث
        var filteredData = ExcelDataList
            .Where(item => item.NotificationType != null && item.NotificationType.Contains("search text"))
            .ToList();

        ExcelDataList = new ObservableCollection<SAPData>(filteredData);
    }

    private void UpdateFilteredData()
    {
        if(ExcelDataList == null || ExcelDataList.Count == 0)
        {
            FilteredExcelDataList = new ObservableCollection<SAPData>();
            // FilteredExcelDataList = new ObservableCollection<SAPData>(ExcelDataList);
            return;
        }

        var filtered = string.IsNullOrWhiteSpace(SearchQuery)
            ? ExcelDataList
            : new ObservableCollection<SAPData>(
                ExcelDataList.Where(data =>
                    (data.Notification.ToString().Contains(SearchQuery)) ||
                    (data.NotificationType?.Contains(SearchQuery) ?? false)));

        FilteredExcelDataList = filtered;
    }

    private async Task LoadExcelAsync()
    {
        try
        {
            IsLoading = true;
            ExcelData = await ExcelHelper.LoadFromExcelAsync(ExcelHelper.FilePathNotifications);

            if(ExcelData != null && ExcelData.Rows.Count > 0)
            {
                ExcelDataList = new ObservableCollection<SAPData>(
                    ExcelData.AsEnumerable().Select(row => new SAPData
                    {
                        Notification = row["Notification"] == DBNull.Value ? 0 : Convert.ToDouble(row["Notification"]) ,
                        NotificationType = row["NotificationType"]?.ToString() ,
                        Region = row["Region"] == DBNull.Value ? 0 : Convert.ToInt32(row["Region"]) ,
                        City = row["City"]?.ToString() ,
                        Street = row["Street"]?.ToString() ,
                        ListName = row["ListName"]?.ToString() ,
                        Telephone = row["Telephone"]?.ToString() ,
                        District = row["District"]?.ToString() ,
                        NotifDate = row["NotifDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["NotifDate"]) ,
                        Description = row["Description"]?.ToString() ,
                        Customer = row["Customer"] == DBNull.Value ? 0 : Convert.ToInt32(row["Customer"]) ,
                        MainWorkCtr = row["MainWorkCtr"]?.ToString() ,
                        SortField = row["SortField"]?.ToString() ,
                        BreakdownDuration = row["BreakdownDuration"] == DBNull.Value ? 0 : Convert.ToInt32(row["BreakdownDuration"]) ,
                        RequiredEnd = row["RequiredEnd"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["RequiredEnd"])
                    }));

                // تم التأكد من أن البيانات تم تحميلها بنجاح
                Summary = $"تم تحميل {ExcelData.Rows.Count} صفوف من ملف Excel.";
            }
            else
            {
                Summary = "لا توجد بيانات لتحميلها من الملف.";
            }
        }
        catch(Exception ex)
        {
            Summary = $"حدث خطأ أثناء تحميل الملف: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanSaveToDatabase()
    {
        return ExcelData != null && ExcelData.Rows.Count > 0;
    }

    private async Task SaveSAPDataToDatabaseAsync()
    {
        if(ExcelData == null || ExcelData.Rows.Count == 0)
        {
            Summary = "يرجى تحميل ملف Excel أولاً.";
            return;
        }

        int addedRows = 0, skippedExists = 0, skippedInvalid = 0;

        foreach(DataRow row in ExcelData.Rows)
        {
            try
            {
                if(!row.Table.Columns.Contains("Notification"))
                {
                    Summary = "بيانات Excel غير صحيحة: العمود 'Notification' مفقود.";
                    return;
                }

                var parameters = PrepareRowParameters(row);

                if(ValidateRow(parameters ,["Notification"]))
                {
                    if(await _databaseHelper.CheckIfSapCodeExistsAsync("SAPDatas" ,Convert.ToDouble(parameters["Notification"])))
                    {
                        skippedExists++;
                    }
                    else
                    {
                        await _databaseHelper.InsertIntoTableAsync("SAPDatas" ,parameters);
                        addedRows++;
                    }
                }
                else
                {
                    skippedInvalid++;
                }
            }
            catch(Exception ex)
            {
                Summary = $"خطأ أثناء الحفظ في الصف ({row.Table.Rows.IndexOf(row) + 1}): {ex.Message}";
            }
        }

        Summary = $"تم الحفظ بنجاح: {addedRows} صفوف.\n" +
                  $"تم تخطي: {skippedExists} صفوف (موجودة بالفعل).\n" +
                  $"تم تخطي: {skippedInvalid} صفوف (بيانات غير صالحة).";
    }  
    
    private async Task SaveEmployeeDataToDatabaseAsync()
    {
        if(ExcelData == null || ExcelData.Rows.Count == 0)
        {
            Summary = "يرجى تحميل ملف Excel أولاً.";
            return;
        }

        int addedRows = 0, skippedExists = 0, skippedInvalid = 0;

        foreach(DataRow row in ExcelData.Rows)
        {
            try
            {
                if(!row.Table.Columns.Contains("Notification"))
                {
                    Summary = "بيانات Excel غير صحيحة: العمود 'Notification' مفقود.";
                    return;
                }

                var parameters = PrepareRowParameters(row);

                if(ValidateRow(parameters ,["Notification"]))
                {
                    if(await _databaseHelper.CheckIfSapCodeExistsAsync("SAPDatas" ,Convert.ToDouble(parameters["Notification"])))
                    {
                        skippedExists++;
                    }
                    else
                    {
                        await _databaseHelper.InsertIntoTableAsync("SAPDatas" ,parameters);
                        addedRows++;
                    }
                }
                else
                {
                    skippedInvalid++;
                }
            }
            catch(Exception ex)
            {
                Summary = $"خطأ أثناء الحفظ في الصف ({row.Table.Rows.IndexOf(row) + 1}): {ex.Message}";
            }
        }

        Summary = $"تم الحفظ بنجاح: {addedRows} صفوف.\n" +
                  $"تم تخطي: {skippedExists} صفوف (موجودة بالفعل).\n" +
                  $"تم تخطي: {skippedInvalid} صفوف (بيانات غير صالحة).";
    }

    private Dictionary<string ,object> PrepareRowParameters(DataRow row)
    {
        return new Dictionary<string ,object>
        {
            { "Notification", row["Notification"] ?? DBNull.Value },
            { "NotificationType", row["NotificationType"] ?? DBNull.Value },
            { "Region", row["Region"] ?? DBNull.Value },
            { "City", row["City"] ?? DBNull.Value },
            { "Street", row["Street"] ?? DBNull.Value },
            { "ListName", row["ListName"] ?? DBNull.Value },
            { "Telephone", row["Telephone"] ?? DBNull.Value },
            { "District", row["District"] ?? DBNull.Value },
            { "NotifDate", row["NotifDate"] ?? DBNull.Value },
            { "Description", row["Description"] ?? DBNull.Value },
            { "Customer", row["Customer"] ?? DBNull.Value },
            { "MainWorkCtr", row["MainWorkCtr"] ?? DBNull.Value },
            { "SortField", row["SortField"] ?? DBNull.Value },
            { "BreakdownDuration", row["BreakdownDuration"] ?? DBNull.Value },
            { "RequiredEnd", row["RequiredEnd"] ?? DBNull.Value }
        };
    }

    private bool ValidateRow(Dictionary<string ,object> row ,string[] requiredColumns)
    {
        foreach(var column in requiredColumns)
        {
            if(!row.ContainsKey(column) || row[column] == DBNull.Value || string.IsNullOrEmpty(row[column]?.ToString()))
            {
                return false;
            }
        }
        return true;
    }

    public async Task LoadEmployeeDataFromExcelAsync(string filePath)
    {
        try
        {
            IsLoading = true;
            ExcelData = await ExcelHelper.LoadFromExcelAsync(filePath);

            if(ExcelData != null && ExcelData.Rows.Count > 0)
            {
                var employeeDataList = ExcelData.AsEnumerable().Select(row => new EmployeeData
                {
                    Code = row["Code"] == DBNull.Value ? 0 : Convert.ToInt32(row["Code"]) ,
                    Name = row["Name"]?.ToString() ,
                    Job = row["Job"]?.ToString() ,
                    Branch = row["Branch"]?.ToString() ,
                    WorkLocation = row["WorkLocation"]?.ToString() ,
                    Department = row["Department"]?.ToString() ,
                    Vendor = row["Vendor"] == DBNull.Value ? 0 : Convert.ToInt32(row["Vendor"])                     
                }).ToList();

                Summary = $"تم تحميل {employeeDataList.Count} صفوف من ملف Excel.";
            }
            else
            {
                Summary = "لا توجد بيانات لتحميلها من الملف.";
            }
        }
        catch(Exception ex)
        {
            Summary = $"حدث خطأ أثناء تحميل الملف: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}