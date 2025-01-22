using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MaintenanceApp.WPF.Helper;

public static class ExcelHelper
{

    private static readonly string ResourcesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
    public static readonly string FilePathNotifications = Path.Combine(ResourcesPath ,"SAPDetails.xlsx");
    public static readonly string FilePathEmployeeDetails = Path.Combine(ResourcesPath ,"EmployeeDetails.xlsx");

    /// <summary>
    /// Load data from an Excel file into a DataTable.
    /// </summary>
    /// <param name="filePath">Path to the Excel file</param>
    /// <returns>DataTable with Excel data</returns>
    public static async Task<DataTable> LoadFromExcelAsync(string filePath)
    {
        try
        {
            if(!File.Exists(filePath)) return null;

            return await Task.Run(() =>
            {
                using var workbook = new ClosedXML.Excel.XLWorkbook(filePath);
                var worksheet = workbook.Worksheets.First();
                var dataTable = new DataTable();

                foreach(var headerCell in worksheet.Row(1).CellsUsed())
                {
                    dataTable.Columns.Add(headerCell.Value.ToString());
                }

                foreach(var dataRow in worksheet.RowsUsed().Skip(1))
                {
                    var row = dataTable.NewRow();
                    for(int i = 0; i < dataRow.CellsUsed().Count(); i++)
                    {
                        row[i] = dataRow.Cell(i + 1).Value;
                    }
                    dataTable.Rows.Add(row);
                }

                return dataTable;
            });
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

   

    public static string SelectExcelFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx" ,
            Title = "اختر ملف Excel"
        };

        return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
    }

}
