using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Input;
using MaintenanceApp.Helper;
using MaintenanceApp.Models;
using MaintenanceApp.WPF.Helper;

namespace MaintenanceApp.WPF.ViewModels;

public class ImportExcelViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private DataTable _excelData;
    public DataTable ExcelData
    {
        get => _excelData;
        set
        {
            _excelData = value;
            OnPropertyChanged(nameof(ExcelData));
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
        }
    }

    public ICommand LoadExcelCommand { get; }
    public ICommand SaveToDatabaseCommand { get; }
    public ICommand CloseCommand { get; }

    private readonly Action _onClose;

    public ImportExcelViewModel(Action onClose)
    {
        LoadExcelCommand = new RelayCommand(LoadExcel);
        SaveToDatabaseCommand = new RelayCommand(
            () => SaveToDatabase(null) , // تحويل الدالة إلى نسخة بدون معلمات
            () => ExcelData != null && ExcelData.Rows.Count > 0 // التحقق من الشرط
        );
        CloseCommand = new RelayCommand(o => onClose());
        _onClose = onClose;
    }

    private void LoadExcel(object obj)
    {
        try
        {
            ExcelData = ExcelHelper.LoadFromExcel(ExcelHelper.filePathNotifications);

            if(ExcelData.Rows.Count <= 0)
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx"
                };

                if(openFileDialog.ShowDialog() == true)
                {
                    ExcelData = ExcelHelper.LoadFromExcel(openFileDialog.FileName);
                }
            }
            Summary = $"تم تحميل {ExcelData.Rows.Count} صفوف من ملف Excel.";
        }
        catch(Exception ex)
        {
            Summary = $"حدث خطأ أثناء تحميل الملف: {ex.Message}";
        }
    }

    private void SaveToDatabase(object obj)
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
                // إعداد القيم مع السماح للقيم الفارغة
                var parameters = new Dictionary<string ,object>
            {
                { "Notification"        , row["Notification"] ?? DBNull.Value },
                { "NotificationType"    , row["NotificationType"] ?? DBNull.Value },
                { "Region"              , row["Region"] ?? DBNull.Value },
                { "City"                , row["City"] ?? DBNull.Value },
                { "Street"              , row["Street"] ?? DBNull.Value },
                { "ListName"            , row["ListName"] ?? DBNull.Value },
                { "Telephone"           , row["Telephone"] ?? DBNull.Value },
                { "District"            , row["District"] ?? DBNull.Value },
                { "NotifDate"           , row["NotifDate"] ?? DBNull.Value },
                { "Description"         , row["Description"] ?? DBNull.Value },
                { "Customer"            , row["Customer"] ?? DBNull.Value },
                { "MainWorkCtr"         , row["MainWorkCtr"] ?? DBNull.Value },
                { "SortField"           , row["SortField"] ?? DBNull.Value },
                { "BreakdownDuration"   , row["BreakdownDuration"] ?? DBNull.Value },
                { "RequiredEnd"         , row["RequiredEnd"] ?? DBNull.Value }
            };

                // التحقق من وجود القيم الأساسية
                if(ValidateRow(parameters ,["Notification"]))
                {
                    //if(DatabaseHelper.CheckIfSapCodeExists("VisitDatas" ,Convert.ToDouble(parameters["Notification"])))
                    //{
                    //    skippedExists++;
                    //}
                    //else
                    //{
                    //    DatabaseHelper.InsertIntoTable("VisitDatas" ,parameters);
                    //    addedRows++;
                    //}
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

    private bool ValidateRow(Dictionary<string ,object> row ,string[] requiredColumns)
    {
        foreach(var column in requiredColumns)
        {
            if(!row.ContainsKey(column) || row[column] == DBNull.Value || string.IsNullOrEmpty(row[column]?.ToString()))
            {
                return false; // صف غير صالح إذا كانت إحدى القيم الأساسية مفقودة.
            }
        }
        return true;
    }

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this ,new PropertyChangedEventArgs(propertyName));
}
