﻿using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MaintenanceApp.WPF.Views;

/// <summary>
/// Interaction logic for ImportSAPDataExcelWindow.xaml
/// </summary>
public partial class ImportSAPDataExcelWindow : UserControl
{
    public ImportSAPDataExcelWindow()
    {
        InitializeComponent();
        DataContext = new ImportSAPDataExcelViewModel(new DatabaseHelper());
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
}