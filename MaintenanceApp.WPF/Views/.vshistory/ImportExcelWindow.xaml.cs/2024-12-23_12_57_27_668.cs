using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MaintenanceApp.WPF.Views;

/// <summary>
/// Interaction logic for ImportExcelWindow.xaml
/// </summary>
public partial class ImportExcelWindow : UserControl
{
    private readonly ImportExcelViewModel viewModel;
    public ImportExcelWindow()
    {
        InitializeComponent();
        viewModel = new ImportExcelViewModel(new DatabaseHelper());
        DataContext = viewModel; // تعيين الـ DataContext}

        CheckData();
    }

    private void CheckData()
    {
        // التحقق بعد تأخير قصير لإعطاء وقت لتحميل البيانات
        Task.Run(async () =>
        {
            await Task.Delay(1000); // تأخير 1 ثانية
            Application.Current.Dispatcher.Invoke(() =>
            {
                if(viewModel.ExcelData != null)
                {
                    MessageBox.Show(viewModel.ExcelData.Rows.Count.ToString());
                }
                else
                {
                    MessageBox.Show("ExcelData = Null");
                }

                if(viewModel.ExcelDataList != null)
                {
                    MessageBox.Show(viewModel.ExcelDataList.Count.ToString());
                }
                else
                {
                    MessageBox.Show("ExcelDataList = Null");
                }
            });
        });
    }
}