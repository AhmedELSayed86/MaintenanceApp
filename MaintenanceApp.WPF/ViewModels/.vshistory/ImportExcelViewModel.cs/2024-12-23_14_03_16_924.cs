using DocumentFormat.OpenXml.Wordprocessing;
using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
            _excelData = value;
           
            UpdateFilteredData();
            SetProperty(ref _excelData ,value);
            ValidateProperty(nameof(Name) ,() => string.IsNullOrWhiteSpace(_excelData.ToString()) ,"Name cannot be empty.");

            //CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    private ObservableCollection<SAPData> _excelDataList;
    public ObservableCollection<SAPData> ExcelDataList
    {
        get => _excelDataList;
        set
        {
            _excelDataList = value;
         
            UpdateFilteredData();
            SetProperty(ref _excelDataList ,value);
            ValidateProperty(nameof(Name) ,() => string.IsNullOrWhiteSpace(_excelDataList.ToString()) ,"Name cannot be empty.");

            //  CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    private ObservableCollection<SAPData> _filteredExcelDataList;
    public ObservableCollection<SAPData> FilteredExcelDataList
    {
        get => _filteredExcelDataList;
        set
        {
            _filteredExcelDataList = value;
            SetProperty(ref _filteredExcelDataList ,value);
            ValidateProperty(nameof(Name) ,() => string.IsNullOrWhiteSpace(_filteredExcelDataList.ToString()) ,"Name cannot be empty.");

            //  CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    private string _searchQuery;
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            SetProperty(ref _searchQuery ,value);
            ValidateProperty(nameof(Name) ,() => string.IsNullOrWhiteSpace(_searchQuery) ,"Name cannot be empty.");

            UpdateFilteredData();
        }
    }

    private string _summary;
    public string Summary
    {
        get => _summary;
        set
        {
            _summary = value;
            SetProperty(ref _summary ,value);
            ValidateProperty(nameof(Name) ,() => string.IsNullOrWhiteSpace(_summary) ,"Name cannot be empty.");

            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
            Debug.WriteLine($"Summary: {Summary}");
        }
    }

    private bool _isLoading = false;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            SetProperty(ref _isLoading ,value);
            ValidateProperty(nameof(Name) ,() => string.IsNullOrWhiteSpace(_isLoading.ToString()) ,"Name cannot be empty.");

        }
    }

    public ICommand LoadExcelCommand { get; }
    public ICommand SaveToDatabaseCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand SearchCommand { get; }


    private readonly Action _onClose;

    public ImportExcelViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
        LoadExcelCommand = new RelayCommand(async o => await LoadExcelAsync());
        SaveToDatabaseCommand = new RelayCommand(async o => await SaveToDatabaseAsync() ,o => CanSaveToDatabase());
        CloseCommand = new RelayCommand(o => _onClose?.Invoke());
        SearchCommand = new RelayCommand(o => ExecuteSearch());
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
        if(ExcelDataList == null)
        {
            FilteredExcelDataList = new ObservableCollection<SAPData>();
            return;
        }

        var filtered = string.IsNullOrWhiteSpace(SearchQuery)
            ? ExcelDataList
            : new ObservableCollection<SAPData>(
                ExcelDataList.Where(data => data.Notification.ToString().Contains(SearchQuery) ||
                                            data.NotificationType.Contains(SearchQuery)));

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
            }

            Summary = $"تم تحميل {ExcelData.Rows.Count} صفوف من ملف Excel.";
            MessageBox.Show($"تم تحميل البيانات بنجاح ExcelData                    : {ExcelData.Rows.Count} صفوف.\n" +
                            $"تم تحميل البيانات بنجاح ExcelDataList              : {ExcelDataList.Count} صفوف.\n" +
                            $"تم تحميل البيانات بنجاح FilteredExcelDataList : {FilteredExcelDataList.Count} صفوف.");
        }
        catch(Exception ex)
        {
            Summary = $"حدث خطأ أثناء تحميل الملف: {ex.Message}";
            MessageBox.Show($"حدث خطأ أثناء تحميل الملف: {ex.Message}");
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

    private async Task SaveToDatabaseAsync()
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
}