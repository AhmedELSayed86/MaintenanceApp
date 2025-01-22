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
    public ImportExcelWindow()
    {
        InitializeComponent();
        DataContext = new ImportExcelViewModel(new DatabaseHelper());

    }

    private void CheckData()
    {
        // التحقق بعد تأخير قصير لإعطاء وقت لتحميل البيانات
        Task.Run(async () =>
        {
            await Task.Delay(1000); // تأخير 1 ثانية
            Application.Current.Dispatcher.Invoke(() =>
            {

            });
        });
    }

    private void Button_Click(object sender ,RoutedEventArgs e)
    {
        ImportExcelViewModel viewModel = new ImportExcelViewModel(new DatabaseHelper());
        DataGridView.ItemsSource = viewModel.FilteredExcelDataList;
        DataGridView1.ItemsSource = viewModel.ExcelDataList;
    }
}