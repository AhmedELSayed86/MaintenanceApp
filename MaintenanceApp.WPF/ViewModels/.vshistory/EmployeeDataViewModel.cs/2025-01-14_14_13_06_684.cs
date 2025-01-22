﻿using MaintenanceApp.WPF.Helper;
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

public class StatusDataViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;

    public ObservableCollection<StatusData> StatusRecords { get; set; } = new();
    private StatusData _selectedStatus;

    public StatusData SelectedStatus
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

    private ObservableCollection<StatusData> _excelDataList;
    public ObservableCollection<StatusData> ExcelDataList
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

    public ICommand LoadExcelStatussCommand { get; }
    public ICommand ImportStatussCommand { get; }
    public ICommand LoadDataCommand { get; }
    public ICommand AddStatusCommand { get; }
    public ICommand UpdateStatusCommand { get; }
    public ICommand DeleteStatusCommand { get; }

    public StatusDataViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;

        LoadExcelStatussCommand = new RelayCommand(async _ =>await LoadStatusDataFromExcelAsync());
        ImportStatussCommand = new RelayCommand(async _ => await SaveStatusDataToDatabaseAsync());
        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
        AddStatusCommand = new RelayCommand(async _ => await AddStatusAsync());
        UpdateStatusCommand = new RelayCommand(async _ => await UpdateStatusAsync());
        DeleteStatusCommand = new RelayCommand(async _ => await DeleteStatusAsync());

        // تحميل البيانات عند إنشاء ViewModel
        _ = LoadDataAsync();
    }
     
    public async Task LoadStatusDataFromExcelAsync()
    {
        try
        {
            IsLoading = true; 
            ExcelData = await ExcelHelper.LoadFromExcelAsync(ExcelHelper.FilePathStatusDetails);

            if(ExcelData != null && ExcelData.Rows.Count > 0)
            {
                StatusRecords = new ObservableCollection<StatusData>(
                    ExcelData.AsEnumerable().Select(row => new StatusData
                    {
                        Code = row["Code"] == DBNull.Value ? 0 : Convert.ToInt32(row["Code"]) ,
                        Name = row["Name"]?.ToString() ,
                        Job = row["Job"]?.ToString() ,
                        Branch = row["Branch"]?.ToString() ,
                        WorkLocation = row["WorkLocation"]?.ToString() ,
                        Department = row["Department"]?.ToString() ,
                        Vendor = row["Vendor"] == DBNull.Value ? 0 : Convert.ToInt32(row["Vendor"])
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
     
    public async Task SaveStatusDataToDatabaseAsync()
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
                    if(await _databaseHelper.CheckIfSapCodeExistsAsync("StatusDatas" ,"Code" ,Convert.ToDouble(parameters["Code"])))
                    {
                        skippedExists++;
                    }
                    else
                    {
                        await _databaseHelper.ExcelInsertIntoTableAsync("StatusDatas" ,parameters);
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
            StatusRecords.Clear();
            var Statuss = await _databaseHelper.GetAllRecordsAsync<StatusData>();
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
            var newStatus = new StatusData
            {
                Code = 345 ,
                Name = "New Status" ,
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

            StatusRecords = new ObservableCollection<StatusData>(filtered);
            OnPropertyChanged(nameof(StatusRecords));
        }
    }
}
