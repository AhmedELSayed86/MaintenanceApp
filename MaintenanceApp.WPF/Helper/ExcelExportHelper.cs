using ClosedXML.Excel;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MaintenanceApp.WPF.Helper;

public static class ExcelExportHelper
{
    /// <summary>
    /// تصدير البيانات إلى ملف Excel مع الحماية
    /// </summary>
    /// <param name="data">البيانات المراد تصديرها</param>
    /// <param name="filePath">مسار حفظ الملف</param>
    /// <param name="password">كلمة المرور لحماية الملف</param>
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

    private static void SetFileProtection(string filePath ,bool isReadOnly)
    {
        new FileInfo(filePath).IsReadOnly = isReadOnly;
    }

    private static void PrepareFile(string filePath)
    {
        SetFileProtection(filePath ,false);
        CloseFile(filePath);
        if(File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private static IXLWorksheet CreateWorksheet(XLWorkbook workbook ,string sheetName)
    {
        var worksheet = workbook.AddWorksheet(sheetName);
        worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        worksheet.RightToLeft = true;
        worksheet.SheetView.SetView(XLSheetViewOptions.PageBreakPreview);
        return worksheet;
    }

    private static void AddHeaders(IXLWorksheet worksheet)
    {
        string[] headers = {
                "Notification", "NotificationType", "Region", "City", "Street", "ListName", "Telephone",
                "District", "NotifDate", "Description", "Customer", "MainWorkCtr", "SortField",
                "BreakdownDuration", "RequiredEnd", "VisitDate", "Technician", "ServiceDetails",
                "Implemented", "DeterminationTechnician"
            };

        for(int col = 0; col < headers.Length; col++)
        {
            var cell = worksheet.Cell(1 ,col + 1);
            cell.Value = headers[col];
            ApplyHeaderStyle(cell);
        }
    }

    private static void AddData(IXLWorksheet worksheet ,dynamic data)
    {
        int row = 2;
        foreach(var record in data)
        {
            worksheet.Cell(row ,1).Value = record.Notification?.ToString() ?? string.Empty;
            worksheet.Cell(row ,2).Value = record.NotificationType ?? string.Empty;
            worksheet.Cell(row ,3).Value = record.Region?.ToString() ?? string.Empty;
            worksheet.Cell(row ,4).Value = record.City ?? string.Empty;
            worksheet.Cell(row ,5).Value = record.Street ?? string.Empty;
            worksheet.Cell(row ,6).Value = record.ListName ?? string.Empty;
            worksheet.Cell(row ,7).Value = record.Telephone ?? string.Empty;
            worksheet.Cell(row ,8).Value = record.District ?? string.Empty;
            worksheet.Cell(row ,9).Value = record.NotifDate?.ToString("dd/MM/yyyy") ?? string.Empty;
            worksheet.Cell(row ,10).Value = record.Description ?? string.Empty;
            worksheet.Cell(row ,11).Value = record.Customer?.ToString() ?? string.Empty;
            worksheet.Cell(row ,12).Value = record.MainWorkCtr ?? string.Empty;
            worksheet.Cell(row ,13).Value = record.SortField ?? string.Empty;
            worksheet.Cell(row ,14).Value = record.BreakdownDuration?.ToString() ?? string.Empty;
            worksheet.Cell(row ,15).Value = record.RequiredEnd?.ToString("dd/MM/yyyy") ?? string.Empty;
            worksheet.Cell(row ,16).Value = record.VisitDate?.ToString("dd/MM/yyyy") ?? string.Empty;
            worksheet.Cell(row ,17).Value = record.Technician ?? string.Empty;
            worksheet.Cell(row ,18).Value = record.ServiceDetails ?? string.Empty;
            worksheet.Cell(row ,19).Value = record.Implemented ?? string.Empty;
            worksheet.Cell(row ,20).Value = record.DeterminationTechnician ?? string.Empty;

            row++;
        }

        worksheet.Columns().AdjustToContents();
    }

    private static void ApplyHeaderStyle(IXLCell cell)
    {
        cell.Style.Font.SetFontSize(14);
        cell.Style.Font.Bold = true;
        cell.Style.Font.FontColor = XLColor.Black;
        cell.Style.Font.FontName = "Times New Roman";
        cell.Style.Fill.BackgroundColor = XLColor.Blue;
        cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        cell.Style.Alignment.WrapText = true;
        //cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).SetBorderColor(XLColor.Green); 
        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Green);
    }

    private static void FormatWorksheet(IXLWorksheet worksheet ,string password)
    {
        worksheet.Protect(password);
        worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        worksheet.PageSetup.Margins.SetTop(0.05).SetBottom(0.05).SetLeft(0.05).SetRight(0.05);
    }

    private static void CloseFile(string filePath)
    {
        foreach(var process in Process.GetProcesses())
        {
            if(!string.IsNullOrEmpty(process.MainWindowTitle) && process.MainWindowTitle.Contains(filePath))
            {
                process.Kill();
                process.WaitForExit();
            }
        }
    }

    private static void OpenFile(string filePath)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = filePath ,
            UseShellExecute = true
        });
    }
}






























/*   public static void ExportToExcelWithProtection(dynamic data ,string filePath ,string password)
{
    try
    {
        // الغاء حماية الملف
        SetFileProtection(filePath ,false);
        // إغلاق الملف المفتوح إذا كان موجودًا
        CloseFile(filePath);

        // حذف الملف إذا كان موجودًا
        if(File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using(var workbook = new XLWorkbook())
        {
            var worksheet = workbook.AddWorksheet("Visit Data Report");

            // تطبيق التنسيق من اليمين لليسار
            worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.RightToLeft = true;
            worksheet.SheetView.SetView(XLSheetViewOptions.PageBreakPreview);
            // إضافة الأعمدة
            string[] headers = {
                "Notification", "NotificationType", "Region", "City", "Street", "ListName", "Telephone", "District", "NotifDate", "Description", "Customer", "MainWorkCtr", "SortField", "BreakdownDuration", "RequiredEnd", "VisitDate", "Technician", "ServiceDetails", "Implemented", "DeterminationTechnician"
            };

            // تنسيق الهيدر
            for(int col = 0; col < headers.Length; col++)
            {
                var cell = worksheet.Cell(1 ,col + 1);
                cell.Value = headers[col];
                cell.Style.Font.FontSize = 14;
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.Black;
                cell.Style.Font.FontName = "Times New Roman";
                cell.Style.Fill.BackgroundColor = XLColor.Blue;
                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                cell.Style.Alignment.WrapText = true;



                // تحديد حدود الخلايا
                cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                cell.Style.Border.BottomBorderColor = XLColor.Green;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // إضافة البيانات
            int row = 2;
            foreach(var record in data)
            {
                worksheet.Cell(row ,1).Value = record.Notification.ToString();
                worksheet.Cell(row ,2).Value = record.NotificationType ?? " ";
                worksheet.Cell(row ,3).Value = record.Region.ToString();
                worksheet.Cell(row ,4).Value = record.City ?? " ";
                worksheet.Cell(row ,5).Value = record.Street ?? " ";
                worksheet.Cell(row ,6).Value = record.ListName ?? " ";
                worksheet.Cell(row ,7).Value = record.Telephone ?? " ";
                worksheet.Cell(row ,8).Value = record.District ?? " ";
                worksheet.Cell(row ,9).Value = record.NotifDate.ToString("dd/MM/yyyy");
                worksheet.Cell(row ,10).Value = record.Description ?? " ";
                worksheet.Cell(row ,11).Value = record.Customer.ToString();
                worksheet.Cell(row ,12).Value = record.MainWorkCtr ?? " ";
                worksheet.Cell(row ,13).Value = record.SortField ?? " ";
                worksheet.Cell(row ,14).Value = record.BreakdownDuration.ToString();
                worksheet.Cell(row ,15).Value = record.RequiredEnd.ToString("dd/MM/yyyy");
                worksheet.Cell(row ,16).Value = record.VisitDate.ToString("dd/MM/yyyy") ?? " ";
                worksheet.Cell(row ,17).Value = record.Technician ?? " ";
                worksheet.Cell(row ,18).Value = record.ServiceDetails ?? " ";
                worksheet.Cell(row ,19).Value = record.Implemented ?? " ";
                worksheet.Cell(row ,20).Value = record.DeterminationTechnician ?? " ";

                row++;
            }

            // تنسيق الأعمدة والصفوف
            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();

            // تطبيق تنسيق الخط لجميع الخلايا
            worksheet.Cells().Style.Font.FontName = "Times New Roman";
            worksheet.Cells().Style.Font.FontSize = 14;
            worksheet.Cells().Style.Font.Bold = true;
            worksheet.Cells().Style.Font.FontColor = XLColor.Black;
            worksheet.Cells().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cells().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Cells().Style.Alignment.WrapText = true;

            // تحديد حدود الخلايا
            worksheet.Cells().Style.Border.TopBorder = XLBorderStyleValues.Thin;
            worksheet.Cells().Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            worksheet.Cells().Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            worksheet.Cells().Style.Border.RightBorder = XLBorderStyleValues.Thin;

            // تفعيل التنسيق كـ "جدول"
            var tableRange = worksheet.Range(1 ,1 ,row - 1 ,headers.Length);
            tableRange.CreateTable();

            // تنسيق الصفوف بالتناوب
            //  worksheet.Rows().Style.Fill.BackgroundColor = XLColor.White;

            for(int i = 3; i < row; i += 2)
            {
                worksheet.Row(i).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }

            // التحكم في ارتفاع الصفوف
            for(int i = 1; i < row; i++)
            {
                worksheet.Row(i).Height = 35; // ارتفاع الصف
            }

            // التحكم في عرض الأعمدة
            worksheet.Column(1).Width = 18;  // Notification 
            worksheet.Column(2).Width = 4;   // NotificationType
            worksheet.Column(3).Width = 3.5;   // Region
            worksheet.Column(4).Width = 8;  // City
            worksheet.Column(5).Width = 35;  // Street
            worksheet.Column(6).Width = 12;  // ListName
            worksheet.Column(7).Width = 17;  // Telephone
            worksheet.Column(8).Width = 19;  // District
            worksheet.Column(9).Width = 14;  // NotifDate
            worksheet.Column(10).Width = 30; // Description
            worksheet.Column(11).Width = 11; // Customer
            worksheet.Column(12).Width = 8;  // MainWorkCtr
            worksheet.Column(13).Width = 15; // SortField
            worksheet.Column(14).Width = 4;  // BreakdownDuration
            worksheet.Column(15).Width = 14; // RequiredEnd
            worksheet.Column(16).Width = 14;  // VisitDate
            worksheet.Column(17).Width = 10; // Technician
            worksheet.Column(18).Width = 35; // ServiceDetails
            worksheet.Column(19).Width = 10; // Implemented
            worksheet.Column(20).Width = 10; // DeterminationTechnician

            // تفعيل ميزة "معاينة فاصل الصفحة"                  
            worksheet.PageSetup.PrintAreas.Add("A1:T17");
            worksheet.PageSetup.AddHorizontalPageBreak(13);
            worksheet.PageSetup.AddVerticalPageBreak(18);

            workbook.PageOptions.ColumnBreaks.Add(14);
            workbook.PageOptions.RowBreaks.Add(13);

            worksheet.PageSetup.FitToPages(1 ,1);

            worksheet.PageSetup.Margins.SetLeft(0.05);
            worksheet.PageSetup.Margins.SetRight(0.05);
            worksheet.PageSetup.Margins.SetTop(0.05);
            worksheet.PageSetup.Margins.SetBottom(0.05);
            worksheet.PageSetup.Margins.SetFooter(0);
            worksheet.PageSetup.Margins.SetHeader(0);

            worksheet.PageSetup.CenterHorizontally = true;

            worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;

            // حماية الورقة
            worksheet.Protect(password);

            // حفظ الملف
            workbook.SaveAs(filePath);
        }

        // حماية الملف ضد النسخ أو التعديل
        SetFileProtection(filePath ,true);

        var printProcess = Process.Start(new ProcessStartInfo { FileName = filePath });
        // طباعة الملف
        // PrintFile(filePath);

        // التأكد من إغلاق الملف قبل حذفه 
        // CloseFile(filePath);                       
        // if(File.Exists(filePath))
        //    File.Delete(filePath);
    }
    catch(IOException ex)
    {
        Debug.WriteLine($"الملف مفتوح بالفعل! سيتم إغلاق الملف قبل المتابعة.\n{ex.Message}");
        MessageBox.Show("الملف مفتوح بالفعل! سيتم إغلاق الملف قبل المتابعة.");
        // CloseFile(filePath);
        ExportToExcelWithProtection(data ,filePath ,password);
    }
    catch(Exception ex)
    {
        MessageBox.Show(ex.Message);
        Debug.WriteLine($"حدث خطأ أثناء التصدير.\n{ex.Message}");
    }
}

/// <summary>
/// إغلاق الملف إذا كان مفتوحًا
/// </summary>
/// <param name="filePath">مسار الملف</param>
private static void CloseFile(string filePath)
{
    try
    {
        foreach(var process in Process.GetProcesses())
        {
            if(!string.IsNullOrEmpty(process.MainWindowTitle) && process.MainWindowTitle.Contains(filePath))
            {
                process.Kill();
                process.WaitForExit();
            }
        }
    }
    catch(Exception ex)
    {
        Console.WriteLine($"حدث خطأ أثناء إغلاق الملف: {ex.Message}");
    }
}

/// <summary>
/// طباعة الملف
/// </summary>
/// <param name="filePath">مسار الملف</param>
private static void PrintFile(string filePath)
{
    try
    {
        using var printProcess = Process.Start(new ProcessStartInfo
        {
            FileName = filePath ,
            Verb = "print" ,
            CreateNoWindow = true ,
            UseShellExecute = true ,
            WindowStyle = ProcessWindowStyle.Hidden
        });
        printProcess?.WaitForExit();
    }
    catch(Exception ex)
    {
        Console.WriteLine($"خطأ أثناء الطباعة: {ex.Message}");
    }
}


/// <summary>
/// تطبيق الحماية ضد النسخ والتعديل
/// </summary>
/// <param name="filePath">مسار الملف</param>
private static void SetFileProtection(string filePath ,bool isReadOnly)
{
    _ = new FileInfo(filePath)
    {
        IsReadOnly = isReadOnly
    };
}
}























/// <summary>
/// تصدير البيانات إلى ملف Excel مع الحماية
/// </summary>
/// <param name="data">البيانات المراد تصديرها</param>
/// <param name="filePath">مسار حفظ الملف</param>
/// <param name="password">كلمة المرور لحماية الملف</param>
public static void ExportToExcelWithProtection2(dynamic data ,string filePath ,string password)
{
    try
    {
        using(var workbook = new XLWorkbook())
        {
            var worksheet = workbook.AddWorksheet("Visit Data Report");

            // إضافة الأعمدة
            string[] headers = {
                "Notification", "NotificationType", "Region", "City", "Street", "ListName", "Telephone", "District",
                "NotifDate", "Description", "Customer", "MainWorkCtr", "SortField", "BreakdownDuration", "RequiredEnd",
                "VisitDate", "Technician", "ServiceDetails", "Implemented", "DeterminationTechnician"
            };

            for(int col = 0; col < headers.Length; col++)
            {
                var cell = worksheet.Cell(1 ,col + 1);
                cell.Value = headers[col];
                cell.Style.Font.FontSize = 14;
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.Green;
                cell.Style.Font.FontName = "Times New Roman";
                cell.Style.Fill.BackgroundColor = XLColor.Blue;
                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                cell.Style.Alignment.WrapText = true;

                // تحديد حدود الخلايا
                cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            }

            // إضافة البيانات
            int row = 2;
            foreach(var record in data)
            {
                worksheet.Cell(row ,1).Value = record.Notification.ToString();
                worksheet.Cell(row ,2).Value = record.NotificationType ?? "";
                worksheet.Cell(row ,3).Value = record.Region.ToString();
                worksheet.Cell(row ,4).Value = record.City ?? "";
                worksheet.Cell(row ,5).Value = record.Street ?? "";
                worksheet.Cell(row ,6).Value = record.ListName ?? "";
                worksheet.Cell(row ,7).Value = record.Telephone ?? "";
                worksheet.Cell(row ,8).Value = record.District ?? "";
                worksheet.Cell(row ,9).Value = record.NotifDate.ToString("dd/MM/yyyy");
                worksheet.Cell(row ,10).Value = record.Description ?? "";
                worksheet.Cell(row ,11).Value = record.Customer.ToString();
                worksheet.Cell(row ,12).Value = record.MainWorkCtr ?? "";
                worksheet.Cell(row ,13).Value = record.SortField ?? "";
                worksheet.Cell(row ,14).Value = record.BreakdownDuration.ToString();
                worksheet.Cell(row ,15).Value = record.RequiredEnd.ToString("dd/MM/yyyy");
                worksheet.Cell(row ,16).Value = record.VisitDate.ToString("dd/MM/yyyy");
                worksheet.Cell(row ,17).Value = record.Technician ?? "";
                worksheet.Cell(row ,18).Value = record.ServiceDetails ?? "";
                worksheet.Cell(row ,19).Value = record.Implemented ?? "";
                worksheet.Cell(row ,20).Value = record.DeterminationTechnician ?? "";

                row++;
            }

            // تنسيق الأعمدة والصفوف
            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();

            // تفعيل التنسيق كـ "جدول"
            var tableRange = worksheet.Range(1 ,1 ,row - 1 ,headers.Length);
            tableRange.CreateTable();

            // تفعيل ميزة "معاينة فاصل الصفحة"
            //worksheet.PageSetup.PageBreakPreview = true;

            // تنسيق الصفوف بالتناوب
            worksheet.Rows().Style.Fill.BackgroundColor = XLColor.White;
            for(int i = 2; i < row; i += 2)
            {
                worksheet.Row(i).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }

            // التحكم في ارتفاع الصفوف

            for(int i = 0; i < row; i++)
            {
                worksheet.Row(1).Height = 35; // ارتفاع الصف
            }

            // التحكم في عرض الأعمدة
            worksheet.Column(1).Width = 20;  // عرض العمود (1). Notification
            worksheet.Column(2).Width = 5;  // عرض العمود (2). NotificationType
            worksheet.Column(3).Width = 25;  // عرض العمود (3). Region
            worksheet.Column(4).Width = 25;  // عرض العمود (4). City
            worksheet.Column(5).Width = 25;  // عرض العمود (5). Street
            worksheet.Column(6).Width = 25;  // عرض العمود (6). ListName
            worksheet.Column(7).Width = 25;  // عرض العمود (7). Telephone
            worksheet.Column(8).Width = 25;  // عرض العمود (8). District
            worksheet.Column(9).Width = 25;  // عرض العمود (9). NotifDate
            worksheet.Column(10).Width = 25; // عرض العمود (10) Description
            worksheet.Column(11).Width = 25; // عرض العمود (11) Customer
            worksheet.Column(12).Width = 25; // عرض العمود (12) MainWorkCtr
            worksheet.Column(13).Width = 25; // عرض العمود (13) SortField
            worksheet.Column(14).Width = 25; // عرض العمود (14) BreakdownDuration
            worksheet.Column(15).Width = 25; // عرض العمود (15) RequiredEnd
            worksheet.Column(16).Width = 25; // عرض العمود (16) VisitDate
            worksheet.Column(17).Width = 25; // عرض العمود (17) Technician
            worksheet.Column(18).Width = 25; // عرض العمود (18) ServiceDetails
            worksheet.Column(19).Width = 25; // عرض العمود (19) Implemented
            worksheet.Column(20).Width = 25; // عرض العمود (20) DeterminationTechnician

            // حجم الخط ونوعه
            worksheet.Cell(1 ,1).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(1 ,1).Style.Font.FontSize = 14;
            worksheet.Cell(1 ,1).Style.Font.FontColor = XLColor.Red;
            worksheet.Cell(1 ,1).Style.Font.Bold = true;

            // حماية الورقة
            worksheet.Protect(password);

            // حفظ الملف
            workbook.SaveAs(filePath);
        }

        // حماية الملف ضد النسخ أو التعديل
        SetFileProtection2(filePath);

        // طباعة الملف
        Process printProcess = Process.Start(new ProcessStartInfo
        {
            FileName = filePath ,
            Verb = "print" ,
            CreateNoWindow = true ,
            WindowStyle = ProcessWindowStyle.Hidden
        });

        // حذف الملف بعد الطباعة
        printProcess?.WaitForExit();
        if(File.Exists(filePath))
            File.Delete(filePath);
    }
    catch(Exception ex)
    {
        Console.WriteLine($"حدث خطأ أثناء التصدير: {ex.Message}");
    }
}

/// <summary>
/// تطبيق الحماية ضد النسخ والتعديل
/// </summary>
/// <param name="filePath">مسار الملف</param>
private static void SetFileProtection2(string filePath)
{
    FileInfo fileInfo = new(filePath);
    fileInfo.IsReadOnly = true;
}



























//////////////////////////////////////////////////////////////////////////////////











/// <summary>
/// تصدير البيانات إلى ملف Excel مع الحماية
/// </summary>
/// <param name="data">البيانات المراد تصديرها</param>
/// <param name="filePath">مسار حفظ الملف</param>
/// <param name="password">كلمة المرور لحماية الملف</param>
public static void ExportToExcelWithProtection1(dynamic data ,string filePath ,string password)
{
    try
    {
        using(var workbook = new XLWorkbook())
        {
            var worksheet = workbook.AddWorksheet("Visit Data Report");

            // إضافة الأعمدة
            string[] headers = {
                "Notification", "NotificationType", "Region", "City", "Street", "ListName", "Telephone", "District",
                "NotifDate", "Description", "Customer", "MainWorkCtr", "SortField", "BreakdownDuration", "RequiredEnd",
                "VisitDate", "Technician", "ServiceDetails", "Implemented", "DeterminationTechnician"
            };

            for(int col = 0; col < headers.Length; col++)
            {
                worksheet.Cell(1 ,col + 1).Value = headers[col];
                worksheet.Cell(1 ,col + 1).Style.Font.Bold = true;
                worksheet.Cell(1 ,col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Cell(1 ,col + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(1 ,col + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Cell(1 ,col + 1).Style.Alignment.WrapText = true;
            }

            // إضافة البيانات
            int row = 2;
            foreach(var record in data)
            {
                worksheet.Cell(row ,1).Value = record.Notification.ToString();
                worksheet.Cell(row ,2).Value = record.NotificationType ?? "";
                worksheet.Cell(row ,3).Value = record.Region.ToString();
                worksheet.Cell(row ,4).Value = record.City ?? "";
                worksheet.Cell(row ,5).Value = record.Street ?? "";
                worksheet.Cell(row ,6).Value = record.ListName ?? "";
                worksheet.Cell(row ,7).Value = record.Telephone ?? "";
                worksheet.Cell(row ,8).Value = record.District ?? "";
                worksheet.Cell(row ,9).Value = record.NotifDate.ToString("dd/MM/yyyy");
                worksheet.Cell(row ,10).Value = record.Description ?? "";
                worksheet.Cell(row ,11).Value = record.Customer.ToString();
                worksheet.Cell(row ,12).Value = record.MainWorkCtr ?? "";
                worksheet.Cell(row ,13).Value = record.SortField ?? "";
                worksheet.Cell(row ,14).Value = record.BreakdownDuration.ToString();
                worksheet.Cell(row ,15).Value = record.RequiredEnd.ToString("dd/MM/yyyy");
                worksheet.Cell(row ,16).Value = record.VisitDate.ToString("dd/MM/yyyy");
                worksheet.Cell(row ,17).Value = record.Technician ?? "";
                worksheet.Cell(row ,18).Value = record.ServiceDetails ?? "";
                worksheet.Cell(row ,19).Value = record.Implemented ?? "";
                worksheet.Cell(row ,20).Value = record.DeterminationTechnician ?? "";

                row++;
            }

            // تنسيق الأعمدة والصفوف
            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();

            // تفعيل التنسيق كـ "جدول"
            var tableRange = worksheet.Range(1 ,1 ,row - 1 ,headers.Length);
            tableRange.CreateTable();

            // تفعيل ميزة "معاينة فاصل الصفحة"
            // worksheet.PageSetup.PageBreakPreview = true;


            // تنسيق الصفوف بالتناوب
            worksheet.Rows().Style.Fill.BackgroundColor = XLColor.White;
            for(int i = 2; i < row; i += 2)
            {
                worksheet.Row(i).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }

            // حماية الورقة
            worksheet.Protect(password);

            // حفظ الملف
            workbook.SaveAs(filePath);
        }

        // حماية الملف ضد النسخ أو التعديل
        SetFileProtection1(filePath);

        // طباعة الملف
        Process printProcess = Process.Start(new ProcessStartInfo
        {
            FileName = filePath ,
            Verb = "print" ,
            CreateNoWindow = true ,
            WindowStyle = ProcessWindowStyle.Hidden
        });

        // حذف الملف بعد الطباعة
        printProcess?.WaitForExit();
        if(File.Exists(filePath))
            File.Delete(filePath);
    }
    catch(Exception ex)
    {
        Console.WriteLine($"حدث خطأ أثناء التصدير: {ex.Message}");
    }
}

/// <summary>
/// تطبيق الحماية ضد النسخ والتعديل
/// </summary>
/// <param name="filePath">مسار الملف</param>
private static void SetFileProtection1(string filePath)
{
    FileInfo fileInfo = new(filePath);
    fileInfo.IsReadOnly = true;
}
}*/