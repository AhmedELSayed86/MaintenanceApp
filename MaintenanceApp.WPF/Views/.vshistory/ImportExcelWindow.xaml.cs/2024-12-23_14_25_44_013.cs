using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using MaintenanceApp.WPF.ViewModels;
using System;
using System.Collections.ObjectModel;
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

    private async void CheckData()
    {
        await Task.Delay(5000); // تأخير 1 ثانية

        if(viewModel.ExcelDataList != null && viewModel.ExcelDataList.Count > 0)
        {
            viewModel.FilteredExcelDataList = new ObservableCollection<SAPData>(viewModel.ExcelDataList);
        }
    }

    private void Button_Click(object sender ,RoutedEventArgs e)
    {CheckData();
        DataGridView.ItemsSource = viewModel.FilteredExcelDataList;
        
    }
}