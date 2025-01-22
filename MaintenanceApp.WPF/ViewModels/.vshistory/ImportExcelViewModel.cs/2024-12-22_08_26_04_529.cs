﻿using MaintenanceApp.WPF.Helper;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
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
            _excelData = value;
            OnPropertyChanged();
        }
    }

    private string _summary;
    public string Summary
    {
        get => _summary;
        set => SetProperty(ref _summary ,value);
    }

    public ICommand LoadExcelCommand { get; }
    public ICommand SaveToDatabaseCommand { get; }
    public ICommand CloseCommand { get; }

    private readonly Action _onClose;

    public ImportExcelViewModel(IDatabaseHelper databaseHelper ,Action onClose)
    {
        _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
        _onClose = onClose ?? throw new ArgumentNullException(nameof(onClose));

        LoadExcelCommand = new ICommand(async () => await LoadExcelAsync());
        SaveToDatabaseCommand = new ICommand(async () => await SaveToDatabaseAsync() ,CanSaveToDatabase)
                                 .ObservesProperty(() => ExcelData);
        CloseCommand = new ICommand(() => _onClose());
    }

    private async Task LoadExcelAsync()
    {
        try
        {
            ExcelData = await ExcelHelper.LoadFromExcelAsync(ExcelHelper.FilePathNotifications);

            if(ExcelData == null || ExcelData.Rows.Count == 0)
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx"
                };

                if(openFileDialog.ShowDialog() == true)
                {
                    ExcelData = await ExcelHelper.LoadFromExcelAsync(openFileDialog.FileName);
                }
            }

            Summary = $"تم تحميل {ExcelData.Rows.Count} صفوف من ملف Excel.";
        }
        catch(Exception ex)
        {
            Summary = $"حدث خطأ أثناء تحميل الملف: {ex.Message}";
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
                var parameters = PrepareRowParameters(row);

                if(ValidateRow(parameters ,new[] { "Notification" }))
                {
                    if(await _databaseHelper.CheckIfSapCodeExistsAsync("VisitDatas" ,Convert.ToDouble(parameters["Notification"])))
                    {
                        skippedExists++;
                    }
                    else
                    {
                        await _databaseHelper.InsertIntoTableAsync("VisitDatas" ,parameters);
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