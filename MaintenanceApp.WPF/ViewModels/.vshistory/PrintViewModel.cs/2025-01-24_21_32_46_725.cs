using MaintenanceApp.WPF.Controllers;
using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using MaintenanceApp.WPF.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels;

public class PrintViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;
  //  private readonly IMessageService _messageService;
   // private readonly Action _closeAction;
   
    public ObservableCollection<SAPData> SAPDatas { get; set; } = new();

    // Commands
    public ICommand LoadCommand { get; }
    public ICommand PrintCommand { get; }
    public ICommand CloseCommand { get; }

    

    // Constructor
    public PrintViewModel(   )
    {
        IDatabaseHelper databaseHelper = _databaseHelper;
        _databaseHelper = databaseHelper;
        //_messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        //_closeAction = onClose ?? throw new ArgumentNullException(nameof(onClose));

        // Initialize Commands
        LoadCommand = new RelayCommand(async() =>await LoadDataAsync());
        PrintCommand = new RelayCommand( ()=> ExportToExcel());
      
        //CloseCommand = new RelayCommand(o => onClose());

        //_closeAction = onClose;
     }

    private void OpeneImplementedWindow()
    {
        //PrintWindow printWindow = new();
        //printWindow.ShowDialog();
    }

    // Load Data Asynchronously
    private async Task LoadDataAsync()
    {
        try
        {
            IsBusy = true;
            var data = await _databaseHelper.GetAllRecordsAsync<SAPData>();
            SAPDatas.Clear();
            foreach(var item in data)
                SAPDatas.Add(item);
        }
        catch(Exception ex)
        {
           await MessageController.SummaryAsync(ex.Message);
           // _messageService.ShowError($"Error loading data: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Export Data to Excel
    private async void ExportToExcel()
    {
        try
        {
            string password = "Ahmed1435";
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath ,"VisitDataReport.xlsx");

            ExcelExportHelper.ExportToExcelWithProtection(SAPDatas ,filePath ,password);

           // _messageService.ShowInfo($"Report successfully created at:\n{filePath}");
        }
        catch(Exception ex)
        {
            await MessageController.SummaryAsync(ex.Message);
            // _messageService.ShowError($"Error exporting data: {ex.Message}");
        }
    }
}








//public class PrintViewModel : BaseViewModel
//{
//    private readonly IDatabaseHelper _databaseHelper;
//    public ObservableCollection<SAPData> SAPDatas { get; set; } = new();

//    public ICommand LoadCommand { get; }
//    public ICommand PrintCommand { get; }
//    public ICommand CloseCommand { get; }

//    public PrintViewModel(IDatabaseHelper databaseHelper ,Action closeAction)
//    {
//        _databaseHelper = databaseHelper;

//        LoadCommand = new ICommand(async () => await LoadDataAsync());
//        PrintCommand = new ICommand(ExportToExcel);
//        CloseCommand = new ICommand(() => closeAction());
//    }

//    private async Task LoadDataAsync()
//    {
//        try
//        {
//            var data = await _databaseHelper.GetAllRecordsAsync<SAPData>();
//            SAPDatas.Clear();
//            foreach(var item in data)
//                SAPDatas.Add(item);
//        }
//        catch(Exception ex)
//        {
//            MessageBox.Show($"Error loading data: {ex.Message}");
//        }
//    }

//    private void ExportToExcel()
//    {
//        try
//        {
//            string password = "Ahmed1435";
//            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
//            string filePath = Path.Combine(desktopPath ,"VisitDataReport.xlsx");

//            ExcelExportHelper.ExportToExcelWithProtection(SAPDatas ,filePath ,password);

//            MessageBox.Show($"Report successfully created at:\n{filePath}");
//        }
//        catch(Exception ex)
//        {
//            MessageBox.Show($"Error exporting data: {ex.Message}");
//        }
//    }

//    //public PrintViewModel()
//    //{
//    //    LoadCommand = new ICommand(async () => await LoadDataAsync());
//    //    PrintCommand = new RelayCommand(ExportToExcel);
//    //} 
//}







/// <summary>
/// ///////////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>
/// 










/*
private void PrintData(object obj)
{
    try
    {
        var document = CreateDocument();
        var pdfRenderer = new PdfDocumentRenderer(true) { Document = document };
        pdfRenderer.RenderDocument();

        // حفظ وفتح الملف
        SaveAndOpenPDF(pdfRenderer ,"VisitDataReport.pdf");

    }
    catch(Exception ex)
    {
        Debug.WriteLine(ex.Message);
        MessageBox.Show("تعذر فتح الملف. تأكد من عدم فتحه مسبقًا.\n" + ex.Message);
    }
}

private void SaveAndOpenPDF(PdfDocumentRenderer pdfRenderer ,string fileName)
{
    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    string fullPath = System.IO.Path.Combine(desktopPath ,fileName);

    try
    {
        // محاولة الحفظ
        pdfRenderer.PdfDocument.Save(fullPath);
        MessageBox.Show($"تم إنشاء التقرير بنجاح وحفظه في المسار:\n{desktopPath}");
        Debug.WriteLine($"تم إنشاء التقرير بنجاح وحفظه في المسار:\n{desktopPath}");
        // فتح الملف المحفوظ
        Process.Start(new ProcessStartInfo
        {
            FileName = fullPath ,
            UseShellExecute = true
        });
    }
    catch(IOException ex)
    {
        MessageBox.Show("الملف مفتوح بالفعل! سيتم إغلاق الملف قبل المتابعة.");
        CloseFile(fileName);
        SaveAndOpenPDF(pdfRenderer ,fileName); // إعادة المحاولة بعد إغلاق الملف
    }
    catch(Exception ex)
    {
        Debug.WriteLine(ex.Message);
        MessageBox.Show("حدث خطأ أثناء حفظ الملف:\n" + ex.Message);
    }
}

// إغلاق الملف المفتوح
public static void CloseFile(string fileName)
{
    try
    {
        foreach(var process in Process.GetProcesses())
        {
            if(process.MainWindowTitle.Contains(fileName))
            {
                process.Kill(); // إنهاء العملية كما في "Task Manager"
                process.WaitForExit(); // الانتظار حتى يتم الإغلاق بشكل كامل
            }
        }
    }
    catch(Exception ex)
    {
        Console.WriteLine($"حدث خطأ أثناء إغلاق الملف: {ex.Message}");
    }
}

private Document CreateDocument()
{
    var document = new Document();
    var section = document.AddSection();

    AddHeaderAndFooter(section);
    // ضبط الهوامش
    section.PageSetup.TopMargin = Unit.FromMillimeter(3);    // الهامش العلوي
    section.PageSetup.BottomMargin = Unit.FromMillimeter(3); // الهامش السفلي
    section.PageSetup.LeftMargin = Unit.FromMillimeter(3);   // الهامش الأيسر
    section.PageSetup.RightMargin = Unit.FromMillimeter(3);  // الهامش الأيمن

    section.PageSetup.Orientation = Orientation.Landscape;

    var table = CreateTable(section);
    AddTableHeader(table);
    AddTableRows(table);

    return document;
}

private MigraDoc.DocumentObjectModel.Tables.Table CreateTable(MigraDoc.DocumentObjectModel.Section section)
{
    var table = section.AddTable();
    table.Borders.Width = 0.5;

    double[] columnWidths = { 2.5 ,1.5 ,1.5 ,3.0 ,3.5 ,3.0 ,2.5 ,2.0 ,2.0 ,2.5 ,2.0 ,2.0 ,2.0 ,2.0 ,2.0 ,2.0 ,2.0 ,2.0 ,2.0 ,2.0 };
    foreach(var width in columnWidths)
    {
        var column = table.AddColumn(Unit.FromCentimeter(width));
        column.Format.Alignment = ParagraphAlignment.Center;
    }

    return table;
}

private void AddTableHeader(MigraDoc.DocumentObjectModel.Tables.Table table)
{
    var headerRow = table.AddRow();
    headerRow.Shading.Color = Colors.LightGray;
    headerRow.Format.Font.Bold = true;

    var font = new Font("Arial");

    string[] headers = {
            "Notification", "NotificationType", "Region", "City", "Street", "ListName",
            "Telephone", "District", "NotifDate", "Description", "Customer", "MainWorkCtr",
            "SortField", "BreakdownDuration", "RequiredEnd", "VisitDate", "Technician",
            "ServiceDetails", "Implemented", "DeterminationTechnician"
        };

    for(int i = 0; i < headers.Length; i++)
    {
        var paragraph = headerRow.Cells[i].AddParagraph(FixArabicText(headers[i]));
        paragraph.Format.Font = font.Clone();
        paragraph.Format.Alignment = ParagraphAlignment.Center;
    }
}

private void AddTableRows(MigraDoc.DocumentObjectModel.Tables.Table table)
{
    bool isEvenRow = false;

    foreach(var record in RecordsToPrint)
    {
        var row = table.AddRow();
        row.Height = Unit.FromCentimeter(0.8); // التحكم بحجم الصف
        row.Shading.Color = isEvenRow ? Colors.LightBlue : Colors.White;

        //string[] values = {
        //            record.Notification.ToString(),
        //            FixArabicText(record.NotificationType ?? ""),
        //            FixArabicText(record.Region.ToString()),
        //            FixArabicText(record.City ?? ""),
        //            FixArabicText(record.Street ?? ""),
        //            FixArabicText(record.ListName ?? ""),
        //            FixArabicText(record.Telephone ?? ""),
        //            FixArabicText(record.District ?? ""),
        //            record.NotifDate.ToString("dd/MM/yyyy"),
        //            FixArabicText(record.Description ?? ""),
        //            FixArabicText(record.Customer.ToString()),
        //            FixArabicText(record.MainWorkCtr ?? ""),
        //            FixArabicText(record.SortField ?? ""),
        //            record.BreakdownDuration.ToString(),
        //            record.RequiredEnd.ToString("dd/MM/yyyy"),
        //            record.VisitDate.ToString("dd/MM/yyyy"),
        //            FixArabicText(record.Technician ?? ""),
        //            FixArabicText(record.ServiceDetails ?? ""),
        //            FixArabicText(record.Implemented ?? ""),
        //            FixArabicText(record.DeterminationTechnician ?? "")
        //        };

        //for(int i = 0; i < values.Length; i++)
        //{
        //    var paragraph = row.Cells[i].AddParagraph(values[i]);
        //    var font = new Font("Arial") { Size = 9 };
        //    paragraph.Format.Font = font.Clone();
        //    paragraph.Format.Font.Size = 9; // حجم النص
        //    paragraph.Format.Alignment = ParagraphAlignment.Right;
        //}

        isEvenRow = !isEvenRow; // تغيير اللون بالتناوب
    }
}

private void AddHeaderAndFooter(Section section)
{
    // إضافة الهيدر
    var header = section.Headers.Primary.AddParagraph();
    header.AddText($"التقرير - التاريخ: {DateTime.Now:yyyy/MM/dd} الساعة: {DateTime.Now:HH:mm}");
    header.Format.Font.Size = 10;
    header.Format.Alignment = ParagraphAlignment.Center;

    // إضافة الفوتر
    var footer = section.Footers.Primary.AddParagraph();
    footer.AddText($"صفحة: {section.PageSetup.StartingNumber} - التاريخ: {DateTime.Now:yyyy/MM/dd}");
    footer.Format.Font.Size = 8;
    footer.Format.Alignment = ParagraphAlignment.Right;
}

private string FixArabicText(string input)
{
    if(string.IsNullOrEmpty(input))
        return input;

    // إذا كان النص يحتوي فقط على الأحرف الإنجليزية أو الأرقام
    if(System.Text.RegularExpressions.Regex.IsMatch(input ,@"^[a-zA-Z0-9\s]*$"))
        return input;

    return RTLHelper.FixRTLText(input);
}


}
*/