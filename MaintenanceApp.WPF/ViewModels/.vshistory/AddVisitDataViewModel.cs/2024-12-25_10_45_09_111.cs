﻿using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels;

public class AddVisitDataViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;

    public ObservableCollection<SAPData> SAPDataRecords { get; set; } = new();
    public ObservableCollection<VisitData> VisitDataRecords { get; set; } = new();

    private SAPData _selectedSAPData;
    public SAPData SelectedSAPData
    {
        get => _selectedSAPData;
        set
        {
            _selectedSAPData = value;
            OnPropertyChanged(nameof(SelectedSAPData));
            LoadVisitData();
        }
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddSAPDataCommand { get; }
    public ICommand AddVisitDataCommand { get; }
    public ICommand DeleteVisitDataCommand { get; }
    public ICommand UpdateVisitDataCommand { get; }

    public SAPDataViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;

        LoadDataCommand = new RelayCommand(async o => await LoadDataAsync());
        AddSAPDataCommand = new RelayCommand(async o => await AddSAPDataAsync());
        AddVisitDataCommand = new RelayCommand(async o => await AddVisitDataAsync());
        DeleteVisitDataCommand = new RelayCommand(async o => await DeleteVisitDataAsync());
        UpdateVisitDataCommand = new RelayCommand(async o => await UpdateVisitDataAsync());
    }

    private async Task LoadDataAsync()
    {
        SAPDataRecords.Clear();
        var sapDataList = await _databaseHelper.GetAllRecordsAsync<SAPData>();

        foreach(var sap in sapDataList)
        {
            var visits = await _databaseHelper.GetRecordsByForeignKeyAsync<VisitData>("Notification" ,sap.Notification);
            sap.Visits = new ObservableCollection<VisitData>(visits);
            SAPDataRecords.Add(sap);
        }
    }

    private void LoadVisitData()
    {
        if(SelectedSAPData != null)
        {
            VisitDataRecords = new ObservableCollection<VisitData>(SelectedSAPData.Visits);
        }
    }

    private async Task AddSAPDataAsync()
    {
        var newSAPData = new SAPData
        {
            Notification = new Random().NextDouble() ,
            NotificationType = "New Type" ,
            NotifDate = DateTime.Now ,
            City = "New City"
        };

        await _databaseHelper.InsertRecordAsync(newSAPData);
        SAPDataRecords.Add(newSAPData);
    }

    private async Task AddVisitDataAsync()
    {
        if(SelectedSAPData == null) return;

        var newVisitData = new VisitData
        {
            Notification = SelectedSAPData.Notification ,
            VisitDate = DateTime.Now ,
            Technician = "New Technician" ,
            ServiceDetails = "New Service" ,
            Paid = 0 ,
            Unpaid = 0
        };

        await _databaseHelper.InsertRecordAsync(newVisitData);
        SelectedSAPData.Visits.Add(newVisitData);
        LoadVisitData();
    }

    private async Task DeleteVisitDataAsync()
    {
        if(VisitDataRecords.Count == 0) return;

        var visitToDelete = VisitDataRecords.FirstOrDefault();
        if(visitToDelete != null)
        {
            await _databaseHelper.DeleteRecordAsync(visitToDelete);
            SelectedSAPData.Visits.Remove(visitToDelete);
            LoadVisitData();
        }
    }

    private async Task UpdateVisitDataAsync()
    {
        if(VisitDataRecords.Count == 0) return;

        var visitToUpdate = VisitDataRecords.FirstOrDefault();
        if(visitToUpdate != null)
        {
            visitToUpdate.Technician = "Updated Technician";
            await _databaseHelper.UpdateRecordAsync(visitToUpdate);
            LoadVisitData();
        }
    }
}






/*
public class AddVisitDataViewModel : BaseViewModel
{
private readonly IDatabaseHelper _databaseHelper;

// مجموعة البيانات الرئيسية
public ObservableCollection<SAPData> SAPDataRecords { get; set; } = new();
public ObservableCollection<SparePartData> SparePartDataRecords { get; set; } = new();

// الكيان الجديد
private SAPData _newRecord = new();
public SAPData NewRecord
{
    get => _newRecord;
    set
    {
        _newRecord = value;
        OnPropertyChanged(nameof(NewRecord));
        CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
    }
}

// البحث
private string _searchQuery;
public string SearchQuery
{
    get => _searchQuery;
    set
    {
        _searchQuery = value;
        OnPropertyChanged(nameof(SearchQuery));
        FilteredRecords.Refresh(); // تحديث الفلترة عند تغيير النص
        CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
    }
}

private ObservableCollection<SAPData> _filteredExcelDataList;
public ObservableCollection<SAPData> FilteredExcelDataList
{
    get => _filteredExcelDataList;
    set
    {
        _filteredExcelDataList = value;
        OnPropertyChanged(nameof(FilteredExcelDataList));
        CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
    }
}

public ICollectionView FilteredRecords { get; }

// الأوامر
public ICommand SaveCommand { get; }
public ICommand CancelCommand { get; }
public ICommand CloseCommand { get; }
public ICommand LoadDataCommand { get; }

public AddVisitDataViewModel(IDatabaseHelper databaseHelper)
{
    _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));

    // الفلترة
    FilteredRecords = CollectionViewSource.GetDefaultView(SAPDataRecords);
    FilteredRecords.Filter = FilterRecords;

    // الأوامر
    SaveCommand = new RelayCommand(o => SaveNewRecord());
    CancelCommand = new RelayCommand(o => CancelOperation());
    CloseCommand = new RelayCommand(o => GotoHome());
    LoadDataCommand = new RelayCommand(async o => await LoadDataAsync());
}

private ObservableCollection<SAPData> _filteredExcelDataList;
public ObservableCollection<SAPData> FilteredExcelDataList
{
    get => _filteredExcelDataList;
    set
    {
        _filteredExcelDataList = value;
        OnPropertyChanged(nameof(FilteredExcelDataList));
        CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
    }
}

private bool FilterRecords(object item)
{
    if(item is SAPData record)
        return string.IsNullOrEmpty(SearchQuery) ||
               record.Notification.ToString().IndexOf(SearchQuery) >= 0;
    return false;
}

private async void SaveNewRecord()
{
    try
    {
        await _databaseHelper.InsertRecordAsync(NewRecord);
        SAPDataRecords.Add(NewRecord);
        NewRecord = new SAPData();
    }
    catch(Exception ex)
    {
        MessageBox.Show($"Error saving record: {ex.Message}");
    }
}

private void CancelOperation() => NewRecord = new SAPData();

private void GotoHome()
{
    MainViewModel mainViewModel = Application.Current.Windows.OfType<MainViewModel>().FirstOrDefault() ?? new MainViewModel();
    mainViewModel.OpenHomeCommand.Execute(this);
}

/// <summary>
/// تحميل البيانات من جدولين في قاعدة البيانات.
/// </summary>
private async Task LoadDataAsync()
{
    try
    {
        // تحميل البيانات من الجدول الأول
        var sapDataRecords = await _databaseHelper.GetAllRecordsAsync<SAPData>();
        SAPDataRecords.Clear();
        foreach(var record in sapDataRecords)
        {
            SAPDataRecords.Add(record);
        }

        // تحميل البيانات من الجدول الثاني
        var anotherTableRecords = await _databaseHelper.GetAllRecordsAsync<SparePartData>();
        SparePartDataRecords.Clear();
        foreach(var record in anotherTableRecords)
        {
            SparePartDataRecords.Add(record);
        }
        MessageBox.Show($"SAPDataRecords.Count: {SAPDataRecords.Count}\n" + $"SparePartDataRecords.Count: {SparePartDataRecords.Count}\n");
    }
    catch(Exception ex)
    {
        MessageBox.Show($"Error loading data: {ex.Message}");
    }
}


// الأوامر       
public ICommand DeleteCommand { get; }

private void DeleteRecord(object parameter)
{
    if(parameter is SAPData record)
        SAPDataRecords.Remove(record);
}
}
*/