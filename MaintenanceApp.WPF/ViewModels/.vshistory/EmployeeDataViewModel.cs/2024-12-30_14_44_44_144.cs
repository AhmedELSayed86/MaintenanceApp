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

        LoadExcelEmployeesCommand = new RelayCommand(async _ => await LoadEmployeesFromExcelAsync());
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
        ImportExcelViewModel importExcelViewModel = new ImportExcelViewModel(_databaseHelper);
        await importExcelViewModel.SaveSAPDataToDatabaseCommandAsync();
        Summary = importExcelViewModel.Summary;
        _ = LoadDataAsync();
    }

    private async Task LoadEmployeesFromExcelAsync()
    {
        ImportExcelViewModel importExcelViewModel = new ImportExcelViewModel(_databaseHelper);
        await importExcelViewModel.LoadEmployeeDataFromExcelAsync(ExcelHelper.FilePathEmployeeDetails);
        Summary = importExcelViewModel.Summary;
        _ = LoadDataAsync();
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
