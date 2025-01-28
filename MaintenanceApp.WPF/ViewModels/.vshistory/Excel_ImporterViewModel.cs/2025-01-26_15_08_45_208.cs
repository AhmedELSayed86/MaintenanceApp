using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace MaintenanceApp.WPF.ViewModels;

public class Excel_ImporterViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;

    public ObservableCollection<Status> StatusRecords { get; set; } = new();
    private Status _selectedStatus;

    public Status SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            _selectedStatus = value; //UpdateFilteredData();
            OnPropertyChanged(nameof(SelectedStatus));
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    private DataTable _excelData;
    public DataTable ExcelData
    {
        get => _excelData;
        set
        {
            _excelData = value;
            OnPropertyChanged(nameof(ExcelData));// UpdateFilteredData();
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    private ObservableCollection<Status> _excelDataList;
    public ObservableCollection<Status> ExcelDataList
    {
        get => _excelDataList;
        set
        {
            _excelDataList = value; //UpdateFilteredData();
            OnPropertyChanged(nameof(ExcelDataList));
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
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
            OnPropertyChanged(nameof(SearchKeyword));
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

    public ICommand LoadExcelStatusCommand { get; }
    public ICommand ImportStatusCommand { get; }
    public ICommand LoadDataCommand { get; }
    public ICommand AddStatusCommand { get; }
    public ICommand UpdateStatusCommand { get; }
    public ICommand DeleteStatusCommand { get; }

    public Excel_ImporterViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;

        LoadExcelStatusCommand = new RelayCommand(async _ => await LoadStatusFromExcelAsync());
        ImportStatusCommand = new RelayCommand(async _ => await SaveStatusToDatabaseAsync());
        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
        AddStatusCommand = new RelayCommand(async _ => await AddStatusAsync());
        UpdateStatusCommand = new RelayCommand(async _ => await UpdateStatusAsync());
        DeleteStatusCommand = new RelayCommand(async _ => await DeleteStatusAsync());

        // تحميل البيانات عند إنشاء ViewModel
        _ = LoadDataAsync();
        await LogInfoAsync("تم تهيئة Excel_ImporterViewModel بنجاح.");
    }

    public async Task LoadStatusFromExcelAsync()
    {
        try
        {
            IsLoading = true;
            ExcelData = await ExcelHelper.LoadFromExcelAsync(ExcelHelper.FilePathStatusDetails);

            if(ExcelData != null && ExcelData.Rows.Count > 0)
            {
                StatusRecords = new ObservableCollection<Status>(
                    ExcelData.AsEnumerable().Select(row => new Status
                    {
                        Name = row["Name"]?.ToString() ,
                        DiscriptionAR = row["DiscriptionAR"]?.ToString() ,
                        DiscriptionEN = row["DiscriptionEN"]?.ToString() ,
                        SAPNO = Convert.ToInt32(row["SAPNO"]?.ToString())
                    }));
                OnPropertyChanged(nameof(StatusRecords));
                // تم التأكد من أن البيانات تم تحميلها بنجاح
                Summary = $"تم تحميل {ExcelData.Rows.Count} صفوف من ملف اكسيل.({StatusRecords.Count})";
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

    public async Task SaveStatusToDatabaseAsync()
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
                if(!row.Table.Columns.Contains("Name"))
                {
                    Summary = "بيانات Excel غير صحيحة: العمود 'Code' مفقود.";
                    return;
                }

                var parameters = PrepareRowParameters(row);

                if(ValidateRow(parameters ,["Name"]))
                {
                    if(await _databaseHelper.CheckIfSapCodeExistsAsync("Statuses" ,"Name" ,parameters["Name"].ToString()))
                    {
                        skippedExists++;
                    }
                    else
                    {
                        await _databaseHelper.ExcelInsertIntoTableAsync("Statuses" ,parameters);
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
        { "Name" , row["Name"] ?? DBNull.Value },
        { "DiscriptionAR" , row["DiscriptionAR"] ?? DBNull.Value },
        { "DiscriptionEN" , row["DiscriptionEN"] ?? DBNull.Value },
        { "SAPNO" , row["SAPNO"] ?? DBNull.Value }
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

    private async Task LoadDataAsync()
    {
        try
        {
            StatusRecords.Clear();
            var Statuss = await _databaseHelper.GetAllRecordsAsync<Status>();
            foreach(var Status in Statuss)
            {
                StatusRecords.Add(Status);
            }
            Summary = $"تم تحميل {StatusRecords.Count} صفوف من ملف Excel.";
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error loading Status data: {ex.Message}");
        }
    }

    private async Task AddStatusAsync()
    {
        try
        {
            var newStatus = new Status
            {
                Name = "New Status" ,
                DiscriptionAR = "Job Title" ,
                DiscriptionEN = "Branch Name" ,
                SAPNO = 1 ,
                CreatedOn = DateTime.Now ,
                ChangeOn = DateTime.Now ,
                CreatedBy = 1 ,
                ChangeBy = 1 ,
                Notes = "New notes"
            };

            await _databaseHelper.InsertRecordAsync(newStatus);
            StatusRecords.Add(newStatus);
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error adding Status: {ex.Message}");
        }
    }

    private async Task UpdateStatusAsync()
    {
        if(SelectedStatus == null) return;

        try
        {
            SelectedStatus.ChangeOn = DateTime.Now;
            SelectedStatus.ChangeBy = 1; // معرف المستخدم الذي قام بالتعديل
            await _databaseHelper.UpdateRecordAsync(SelectedStatus);
            MessageBox.Show("Status updated successfully.");
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error updating Status: {ex.Message}");
        }
    }

    private async Task DeleteStatusAsync()
    {
        if(SelectedStatus == null) return;

        try
        {
            await _databaseHelper.DeleteRecordAsync(SelectedStatus);
            StatusRecords.Remove(SelectedStatus);
            SelectedStatus = null;
            MessageBox.Show("Status deleted successfully.");
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error deleting Status: {ex.Message}");
        }
    }

    private void ApplyFilter()
    {
        if(string.IsNullOrWhiteSpace(SearchKeyword))
        {
            OnPropertyChanged(nameof(StatusRecords)); // إظهار جميع البيانات
        }
        else
        {
            var filtered = StatusRecords
                .Where(e => e.Name?.Contains(SearchKeyword) == true)
                .ToList();

            StatusRecords = new ObservableCollection<Status>(filtered);
            OnPropertyChanged(nameof(StatusRecords));
        }
    }
}
