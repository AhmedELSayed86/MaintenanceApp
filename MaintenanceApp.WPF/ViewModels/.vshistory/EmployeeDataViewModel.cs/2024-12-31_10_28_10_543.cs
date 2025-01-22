using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels;

public class EmployeeDataViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;

    public ObservableCollection<EmployeeData> EmployeeRecords { get; set; } = new();
    private EmployeeData _selectedEmployee;

    public EmployeeData SelectedEmployee
    {
        get => _selectedEmployee;
        set
        {
            _selectedEmployee = value; UpdateFilteredData();
            OnPropertyChanged(nameof(SelectedEmployee));
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
            OnPropertyChanged(nameof(ExcelData)); UpdateFilteredData();
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    private ObservableCollection<EmployeeData> _excelDataList;
    public ObservableCollection<EmployeeData> ExcelDataList
    {
        get => _excelDataList;
        set
        {
            _excelDataList = value; UpdateFilteredData();
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

    public ICommand LoadExcelEmployeesCommand { get; }
    public ICommand ImportEmployeesCommand { get; }
    public ICommand LoadDataCommand { get; }
    public ICommand AddEmployeeCommand { get; }
    public ICommand UpdateEmployeeCommand { get; }
    public ICommand DeleteEmployeeCommand { get; }

    public EmployeeDataViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;

        LoadExcelEmployeesCommand = new RelayCommand(async _ =>await LoadEmployeeDataFromExcelAsync());
        ImportEmployeesCommand = new RelayCommand(async _ => await SaveEmployeeDataToDatabaseAsync());
        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
        AddEmployeeCommand = new RelayCommand(async _ => await AddEmployeeAsync());
        UpdateEmployeeCommand = new RelayCommand(async _ => await UpdateEmployeeAsync());
        DeleteEmployeeCommand = new RelayCommand(async _ => await DeleteEmployeeAsync());

        // تحميل البيانات عند إنشاء ViewModel
        _ = LoadDataAsync();
    }

    private void UpdateFilteredData()
    {
        if(ExcelDataList == null || ExcelDataList.Count == 0)
        {
            EmployeeRecords = new ObservableCollection<EmployeeData>();
            // FilteredExcelDataList = new ObservableCollection<SAPData>(ExcelDataList);
            return;
        }

        var filtered = string.IsNullOrWhiteSpace(SearchKeyword)
            ? ExcelDataList
            : new ObservableCollection<EmployeeData>(
                ExcelDataList.Where(data =>
                    (data.Code.ToString().Contains(SearchKeyword)) ||
                    (data.Name?.Contains(SearchKeyword) ?? false)));

        EmployeeRecords = filtered;
    }

    public async Task LoadEmployeeDataFromExcelAsync()
    {
        try
        {
            IsLoading = true; 
            ExcelData = await ExcelHelper.LoadFromExcelAsync(ExcelHelper.FilePathEmployeeDetails);

            if(ExcelData != null && ExcelData.Rows.Count > 0)
            {
                ExcelDataList = new ObservableCollection<EmployeeData>(
                    ExcelData.AsEnumerable().Select(row => new EmployeeData
                    {
                        Code = row["Code"] == DBNull.Value ? 0 : Convert.ToInt32(row["Code"]) ,
                        Name = row["Name"]?.ToString() ,
                        Job = row["Job"]?.ToString() ,
                        Branch = row["Branch"]?.ToString() ,
                        WorkLocation = row["WorkLocation"]?.ToString() ,
                        Department = row["Department"]?.ToString() ,
                        Vendor = row["Vendor"] == DBNull.Value ? 0 : Convert.ToInt32(row["Vendor"])
                    }));
                
                // تم التأكد من أن البيانات تم تحميلها بنجاح
                Summary = $"تم تحميل {ExcelData.Rows.Count} صفوف من ملف Excel.({EmployeeRecords.Count})";
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
     
    public async Task SaveEmployeeDataToDatabaseAsync()
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
                if(!row.Table.Columns.Contains("Code"))
                {
                    Summary = "بيانات Excel غير صحيحة: العمود 'Code' مفقود.";
                    return;
                }

                var parameters = PrepareRowParameters(row);

                if(ValidateRow(parameters ,["Code"]))
                {
                    if(await _databaseHelper.CheckIfSapCodeExistsAsync("EmployeeDatas" ,"Code" ,Convert.ToDouble(parameters["Code"])))
                    {
                        skippedExists++;
                    }
                    else
                    {
                        await _databaseHelper.InsertIntoTableAsync("EmployeeDatas" ,parameters);
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
            { "Code" , row["Code"] ?? DBNull.Value },
            { "Name" , row["Name"] ?? DBNull.Value },
            { "Job" , row["Job"] ?? DBNull.Value },
            { "Branch" , row["Branch"] ?? DBNull.Value },
            { "WorkLocation" , row["WorkLocation"] ?? DBNull.Value },
            { "Department" , row["Department"] ?? DBNull.Value },
            { "Vendor" , row["Vendor"] ?? DBNull.Value }
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
            EmployeeRecords.Clear();
            var employees = await _databaseHelper.GetAllRecordsAsync<EmployeeData>();
            foreach(var employee in employees)
            {
                EmployeeRecords.Add(employee);
            }
            Summary = $"تم تحميل {EmployeeRecords.Count} صفوف من ملف Excel.";
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error loading employee data: {ex.Message}");
        }
    }

    private async Task AddEmployeeAsync()
    {
        try
        {
            var newEmployee = new EmployeeData
            {
                Code = 345 ,
                Name = "New Employee" ,
                Job = "Job Title" ,
                Branch = "Branch Name" ,
                WorkLocation = "Work Location" ,
                Department = "Department Name" ,
                Vendor = 12345 ,
                CreatedOn = DateTime.Now ,
                ChangeOn = DateTime.Now ,
                CreatedBy = 1 ,
                ChangeBy = 1 ,
                Notes = "New notes"
            };

            await _databaseHelper.InsertRecordAsync(newEmployee);
            EmployeeRecords.Add(newEmployee);
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error adding employee: {ex.Message}");
        }
    }

    private async Task UpdateEmployeeAsync()
    {
        if(SelectedEmployee == null) return;

        try
        {
            SelectedEmployee.ChangeOn = DateTime.Now;
            SelectedEmployee.ChangeBy = 1; // معرف المستخدم الذي قام بالتعديل
            await _databaseHelper.UpdateRecordAsync(SelectedEmployee);
            MessageBox.Show("Employee updated successfully.");
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error updating employee: {ex.Message}");
        }
    }

    private async Task DeleteEmployeeAsync()
    {
        if(SelectedEmployee == null) return;

        try
        {
            await _databaseHelper.DeleteRecordAsync(SelectedEmployee);
            EmployeeRecords.Remove(SelectedEmployee);
            SelectedEmployee = null;
            MessageBox.Show("Employee deleted successfully.");
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error deleting employee: {ex.Message}");
        }
    }

    private void ApplyFilter()
    {
        if(string.IsNullOrWhiteSpace(SearchKeyword))
        {
            OnPropertyChanged(nameof(EmployeeRecords)); // إظهار جميع البيانات
        }
        else
        {
            var filtered = EmployeeRecords
                .Where(e => e.Name?.Contains(SearchKeyword) == true)
                .ToList();

            EmployeeRecords = new ObservableCollection<EmployeeData>(filtered);
            OnPropertyChanged(nameof(EmployeeRecords));
        }
    }
}
