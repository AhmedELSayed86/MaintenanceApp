using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System;
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
            _selectedEmployee = value;
            OnPropertyChanged(nameof(SelectedEmployee));
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
        }
    }

    public ICommand ImportEmployeesCommand { get; }
    public ICommand LoadDataCommand { get; }
    public ICommand AddEmployeeCommand { get; }
    public ICommand UpdateEmployeeCommand { get; }
    public ICommand DeleteEmployeeCommand { get; }

    public EmployeeDataViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;

        ImportEmployeesCommand = new RelayCommand(async _ => await ImportEmployeesFromExcelAsync());
        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
        AddEmployeeCommand = new RelayCommand(async _ => await AddEmployeeAsync());
        UpdateEmployeeCommand = new RelayCommand(async _ => await UpdateEmployeeAsync());
        DeleteEmployeeCommand = new RelayCommand(async _ => await DeleteEmployeeAsync());

        // تحميل البيانات عند إنشاء ViewModel
        _ = LoadDataAsync();
    }

    private async Task ImportEmployeesFromExcelAsync()
    {
        try
        {
            var filePath = ExcelHelper.LoadFromExcelAsync(); // طريقة لاختيار الملف
            if(string.IsNullOrEmpty(filePath)) return;

            var employeeData = await ExcelHelper.LoadFromExcelAsync(filePath);

            if(employeeData == null || employeeData.Rows.Count == 0)
            {
                MessageBox.Show("لم يتم العثور على بيانات في ملف Excel.");
                return;
            }

            foreach(DataRow row in employeeData.Rows)
            {
                var newEmployee = new EmployeeData
                {
                    ID = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]) ,
                    Name = row["Name"]?.ToString() ,
                    Job = row["Job"]?.ToString() ,
                    Branch = row["Branch"]?.ToString() ,
                    WorkLocation = row["WorkLocation"]?.ToString() ,
                    Department = row["Department"]?.ToString() ,
                    Vendor = row["Vendor"] == DBNull.Value ? 0 : Convert.ToInt32(row["Vendor"]) ,
                    CreatedOn = DateTime.Now ,
                    CreatedBy = 1 , // قم بتعديل القيم الافتراضية
                    Notes = row["Notes"]?.ToString()
                };

                await _databaseHelper.InsertRecordAsync(newEmployee);
            }

            MessageBox.Show("تم استيراد بيانات الموظفين بنجاح.");
        }
        catch(Exception ex)
        {
            MessageBox.Show($"خطأ أثناء استيراد بيانات الموظفين: {ex.Message}");
        }
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
                .Where(e => e.Name?.Contains(SearchKeyword ) == true)
                .ToList();

            EmployeeRecords = new ObservableCollection<EmployeeData>(filtered);
            OnPropertyChanged(nameof(EmployeeRecords));
        }
    }
}
