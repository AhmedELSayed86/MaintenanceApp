using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace MaintenanceApp.WPF.Helper;

public static class ExcelHelper
{

    private static readonly string ResourcesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
    public static readonly string FilePathNotifications = Path.Combine(ResourcesPath ,"NotificationsDetails.xlsx");

    /// <summary>
    /// Load data from an Excel file into a DataTable.
    /// </summary>
    /// <param name="filePath">Path to the Excel file</param>
    /// <returns>DataTable with Excel data</returns>
    public static DataTable LoadFromExcel(string filePath)
    {
        try
        {
            if(!File.Exists(filePath)) return null;

            var dataTable = new DataTable();
            using(var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1); // Read the first worksheet
                bool isFirstRow = true;

                foreach(var row in worksheet.Rows())
                {
                    if(isFirstRow)
                    {
                        foreach(var cell in row.Cells())
                            dataTable.Columns.Add(cell.Value.ToString());
                        isFirstRow = false;
                    }
                    else
                    {
                        dataTable.Rows.Add(row.Cells().Select(c => c.Value.ToString()).ToArray());
                    }
                }
            }
            return dataTable;
        }
        catch(Exception ex)
        {
            MessageBox.Show("Error loading Excel file: " + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Import visit data from an Excel file.
    /// </summary>
    /// <returns>True if the import succeeds, otherwise false</returns>
    public static bool ImportVisitDatasExcelFile()
    {
        return ImportVisitDatasData(FilePathNotifications ,"VisitDatas");
    }

    /// <summary>
    /// Import data from an Excel file into a database.
    /// </summary>
    /// <param name="excelFilePath">Path to the Excel file</param>
    /// <param name="sheetName">Worksheet name</param>
    /// <returns>True if the import succeeds, otherwise false</returns>
    private static bool ImportVisitDatasData(string excelFilePath ,string sheetName)
    {
        if(!File.Exists(excelFilePath))
        {
            MessageBox.Show("File not found: " + excelFilePath);
            return false;
        }

        try
        {
            using var workbook = new XLWorkbook(excelFilePath);
            var worksheet = workbook.Worksheet(sheetName);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip the first row

            int skippedDueToExistence = 0;
            int skippedDueToInvalidData = 0;
            int addedData = 0;

            foreach(var row in rows)
            {
                var sapCode = row.Cell(1).GetValue<string>();

                var parameters = new Dictionary<string ,object>
                    {
                        { "SapCode", sapCode },
                        { "PartNo", row.Cell(2).GetValue<string>() },
                        { "Group_", row.Cell(3).GetValue<string>() },
                        { "Model", row.Cell(4).GetValue<string>() },
                        { "DescriptionAR", row.Cell(5).GetValue<string>() },
                        { "DescriptionEN", row.Cell(6).GetValue<string>() },
                        { "C1", row.Cell(7).GetValue<string>() },
                        { "C2", row.Cell(8).GetValue<string>() },
                        { "IsDamaged", row.Cell(9).GetValue<bool>() }
                    };

                if(ValidateRow(parameters))
                {
                    // DatabaseHelper.InsertRecordAsync(parameters, "VisitDatas");
                    addedData++;
                }
                else
                {
                    skippedDueToInvalidData++;
                }
            }

            MessageBox.Show($"Added: {addedData}, Skipped due to existence: {skippedDueToExistence}, Skipped due to invalid data: {skippedDueToInvalidData}");
            return true;
        }
        catch(Exception ex)
        {
            MessageBox.Show("Error processing the file: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Validate row data before processing.
    /// </summary>
    /// <param name="parameters">Row data as dictionary</param>
    /// <returns>True if valid, otherwise false</returns>
    private static bool ValidateRow(Dictionary<string ,object> parameters)
    {
        return parameters.All(param => param.Value != null && !string.IsNullOrEmpty(param.Value.ToString()));
    }
}

public static class ExcelExportHelper
{
    /// <summary>
    /// Exports data to an Excel file with protection.
    /// </summary>
    /// <param name="data">Data to export</param>
    /// <param name="filePath">File path to save the Excel file</param>
    /// <param name="password">Password for file protection</param>
    public static void ExportToExcelWithProtection(dynamic data ,string filePath ,string password)
    {
        try
        {
            // Remove file protection
            SetFileProtection(filePath ,false);

            // Close the file if it's already open
            CloseFile(filePath);

            // Delete the file if it exists
            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using(var workbook = new XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet("Visit Data Report");

                // Apply right-to-left alignment
                worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.RightToLeft = true;

                // Add headers
                string[] headers = { "Notification" ,"NotificationType" ,"Region" ,"City" ,"Street" ,"ListName" ,"Telephone" ,"District" ,"NotifDate" ,"Description" ,"Customer" ,"MainWorkCtr" ,"SortField" ,"BreakdownDuration" ,"RequiredEnd" ,"VisitDate" ,"Technician" ,"ServiceDetails" ,"Implemented" ,"DeterminationTechnician" };

                for(int col = 0; col < headers.Length; col++)
                {
                    var cell = worksheet.Cell(1 ,col + 1);
                    cell.Value = headers[col];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontColor = XLColor.Black;
                    cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                }

                // Add data
                int row = 2;
                foreach(var record in data)
                {
                    for(int col = 1; col <= headers.Length; col++)
                    {
                        worksheet.Cell(row ,col).Value = record[headers[col - 1]];
                    }
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                // Protect the worksheet
                worksheet.Protect(password);

                // Save the workbook
                workbook.SaveAs(filePath);
            }

            // Set file as read-only
            SetFileProtection(filePath ,true);
        }
        catch(Exception ex)
        {
            MessageBox.Show("Error exporting to Excel: " + ex.Message);
        }
    }

    private static void CloseFile(string filePath)
    {
        // Logic to close an open file
    }

    private static void SetFileProtection(string filePath ,bool isReadOnly)
    {
        var fileInfo = new FileInfo(filePath);
        fileInfo.IsReadOnly = isReadOnly;
    }
}




















/* private static IDatabaseHelper _databaseHelper;
 private static readonly string ResourcesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
 public static readonly string filePathNotifications = Path.Combine(ResourcesPath ,"NotificationsDetails.xlsx");

 public static DataTable LoadFromExcel(string filePath)
 {
     try
     {
         if(!File.Exists(filePath)) return null;

         var dataTable = new DataTable();
         using(var workbook = new XLWorkbook(filePathNotifications))
         {
             var worksheet = workbook.Worksheet(1); // قراءة الورقة الأولى
             bool isFirstRow = true;

             foreach(var row in worksheet.Rows())
             {
                 if(isFirstRow)
                 {
                     foreach(var cell in row.Cells())
                         dataTable.Columns.Add(cell.Value.ToString());
                     isFirstRow = false;
                 }
                 else
                 {
                     dataTable.Rows.Add(row.Cells().Select(c => c.Value.ToString()).ToArray());
                 }
             }
         }
         return dataTable;
     }
     catch(Exception)
     {

         throw;
     }
 }

 public static bool ImportVisitDatasExcelFile()
 {
     return ImportVisitDatasData(filePathNotifications ,"VisitDatas");
 }

 private static bool ImportVisitDatasData(string excelFilePath ,string sheetName)
 {
     if(!File.Exists(excelFilePath))
     {
         MessageBox.Show("الملف غير موجود: " + excelFilePath);
         return false;
     }

     try
     {
         using var workbook = new XLWorkbook(excelFilePath);
         var worksheet = workbook.Worksheet(sheetName);
         var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // تخطي الصف الأول

         int skippedDueToExistence = 0;
         int skippedDueToInvalidData = 0;
         int AddedData = 0;

         foreach(var row in rows)
         {
             var SapCode = row.Cell(1).GetValue<string>();

             //if(DatabaseHelper.CheckIfSapCodeExists("VisitDatas" , Convert.ToInt32(SapCode)))
             //{
             //    skippedDueToExistence++;
             //    continue;
             //}

             var parameters = new Dictionary<string ,object>
                 {
                     { "SapCode", SapCode },
                     { "PartNo", row.Cell(2).GetValue<string>() },
                     { "Group_", row.Cell(3).GetValue<string>() },
                     { "Model", row.Cell(4).GetValue<string>() },
                     { "DescriptionAR", row.Cell(5).GetValue<string>() },
                     { "DescriptionEN", row.Cell(6).GetValue<string>() },
                     { "C1", row.Cell(7).GetValue<string>() },
                     { "C2", row.Cell(8).GetValue<string>() },
                     { "IsDamaged", row.Cell(9).GetValue<bool>() }
                 };

             if(ValidateRow(parameters))
             {
                 //  _databaseHelper.InsertRecordAsync(parameters ,"VisitDatas");
                 AddedData++;
             }
             else
             {
                 skippedDueToInvalidData++;
             }
         }

         //_ = MyMessageService.ShowMessage($"تم أضافة ({AddedData}) صفوف بنجاح،\n" +
         //  $"تم تخطي عدد ({skippedDueToExistence}) صفوف بسبب ان الكود موجود بالفعل،\n" +
         //  $"وتم تخطي عدد ({skippedDueToInvalidData}) صفوف بسبب عدم توافق البيانات." , Brushes.LawnGreen);
         return true;
     }
     catch(Exception)
     {
         //_ = MyMessageService.ShowMessage("حدث خطأ أثناء معالجة الملف: " + ex.Message , Brushes.IndianRed);
         return false;
     }
 }

 private static bool ValidateRow(Dictionary<string ,object> parameters)
 {
     foreach(var param in parameters)
     {
         if(param.Value == null || string.IsNullOrEmpty(param.Value.ToString()))
         {
             return false;
         }
     }
     return true;
 }
}*/