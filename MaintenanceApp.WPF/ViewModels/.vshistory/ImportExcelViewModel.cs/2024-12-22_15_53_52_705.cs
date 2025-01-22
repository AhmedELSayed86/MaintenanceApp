using MaintenanceApp.WPF.Helper;
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
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    private ObservableCollection<dynamic> _excelDataList;
    public ObservableCollection<dynamic> ExcelDataList
    {
        get => _excelDataList;
        set
        {
            _excelDataList = value;
            OnPropertyChanged();
        }
    }

    private string _summary;
    public string Summary
    {
        get => _summary;
        set
        {
            _summary = value;
            OnPropertyChanged();
            Debug.WriteLine($"Summary: {Summary}");
        }
    }

    public ICommand LoadExcelCommand { get; }
    public ICommand SaveToDatabaseCommand { get; }
    public ICommand CloseCommand { get; }

    private readonly Action _onClose;

    public ImportExcelViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
        LoadExcelCommand = new RelayCommand(async o => await LoadExcelAsync());
        SaveToDatabaseCommand = new RelayCommand(async o => await SaveToDatabaseAsync() ,o => CanSaveToDatabase());
        CloseCommand = new RelayCommand(o => _onClose?.Invoke());
    }

    private async Task LoadExcelAsync1()
    {
        try
        {
            ExcelData = await ExcelHelper.LoadFromExcelAsync(ExcelHelper.FilePathNotifications);

            if(ExcelData == null || ExcelData.Rows.Count == 0)
            {
                Summary = "لم يتم العثور على بيانات في الملف.";
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
            MessageBox.Show($"تم تحميل البيانات بنجاح: {ExcelData.Rows.Count} صفوف.");
        }
        catch(Exception ex)
        {
            Summary = $"حدث خطأ أثناء تحميل الملف: {ex.Message}";
            MessageBox.Show($"خطأ: {ex.Message}");
        }
    }

    private async Task LoadExcelAsync()
    {
        try
        {
            ExcelData = await ExcelHelper.LoadFromExcelAsync(ExcelHelper.FilePathNotifications);

            if(ExcelData != null && ExcelData.Rows.Count > 0)
            {
                ExcelDataList = new ObservableCollection<dynamic>(ExcelData.AsEnumerable()
                    .Select(row => new
                    {
                        Notification = row["Notification"] ,
                        NotificationType = row["NotificationType"] ,
                        Region = row["Region"] ,
                        City = row["City"] ,
                        Street = row["Street"] ,
                        ListName = row["ListName"] ,
                        Telephone = row["Telephone"] ,
                        District = row["District"] ,
                        NotifDate = row["NotifDate"] ,
                        Description = row["Description"] ,
                        Customer = row["Customer"] ,
                        MainWorkCtr = row["MainWorkCtr"] ,
                        SortField = row["SortField"] ,
                        BreakdownDuration = row["BreakdownDuration"] ,
                        RequiredEnd = row["RequiredEnd"]
                    }));
            }

            Summary = $"تم تحميل {ExcelData.Rows.Count} صفوف من ملف Excel.";
            MessageBox.Show($"تم تحميل البيانات بنجاح: {ExcelData.Rows.Count} صفوف.");
        }
        catch(Exception ex)
        {
            Summary = $"حدث خطأ أثناء تحميل الملف: {ex.Message}";
            MessageBox.Show(ex.Message);
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
                // تحقق من الأعمدة
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