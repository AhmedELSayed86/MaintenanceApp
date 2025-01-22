using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels;

public class AddVisitDataViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;

    // دمج البيانات من الجدولين
    public ObservableCollection<object> UnifiedData { get; set; } = new();

    private object _selectedRecord;
    public object SelectedRecord
    {
        get => _selectedRecord;
        set
        {
            _selectedRecord = value;
            OnPropertyChanged(nameof(SelectedRecord));
        }
    }

    // الكيان الجديد
    private object _newRecord;
    public object NewRecord
    {
        get => _newRecord;
        set
        {
            _newRecord = value;
            OnPropertyChanged(nameof(NewRecord));
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
            FilteredRecords.Refresh(); // تحديث الفلترة عند تغيير النص
        }
    }

    public ICollectionView FilteredRecords { get; }

    // الأوامر
    public ICommand SaveCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand LoadDataCommand { get; }

    public AddVisitDataViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));

        // الفلترة
        FilteredRecords = CollectionViewSource.GetDefaultView(UnifiedData);
        FilteredRecords.Filter = FilterRecords;

        // الأوامر
        SaveCommand = new RelayCommand(async o => await SaveRecordAsync());
        EditCommand = new RelayCommand(async o => await EditRecordAsync());
        DeleteCommand = new RelayCommand(async o => await DeleteRecordAsync());
        LoadDataCommand = new RelayCommand(async o => await LoadDataAsync());
    }

    private bool FilterRecords(object item)
    {
        if(item is SAPData sapData)
            return string.IsNullOrEmpty(SearchQuery) ||
                   sapData.Notification.ToString().Contains(SearchQuery) ||
                   (sapData.Description?.Contains(SearchQuery) ?? false);

        if(item is SparePartData sparePartData)
            return string.IsNullOrEmpty(SearchQuery) ||
                   sparePartData.SapCode.ToString().Contains(SearchQuery);

        return false;
    }

    private async Task LoadDataAsync()
    {
        try
        {
            UnifiedData.Clear();

            // تحميل البيانات من الجدولين
            var sapDataRecords = await _databaseHelper.GetAllRecordsAsync<SAPData>();
            foreach(var record in sapDataRecords)
                UnifiedData.Add(record);

            var sparePartDataRecords = await _databaseHelper.GetAllRecordsAsync<SparePartData>();
            foreach(var record in sparePartDataRecords)
                UnifiedData.Add(record);

            MessageBox.Show($"Loaded {UnifiedData.Count} records from both tables.");
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error loading data: {ex.Message}");
        }
    }

    private async Task SaveRecordAsync()
    {
        try
        {
            if(NewRecord is SAPData sapData)
            {
                await _databaseHelper.InsertRecordAsync(sapData);
            }
            else if(NewRecord is SparePartData sparePartData)
            {
                await _databaseHelper.InsertRecordAsync(sparePartData);
            }

            UnifiedData.Add(NewRecord);
            NewRecord = null;
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error saving record: {ex.Message}");
        }
    }

    private async Task EditRecordAsync()
    {
        try
        {
            if(SelectedRecord is SAPData sapData)
            {
                await _databaseHelper.UpdateRecordAsync(sapData);
            }
            else if(SelectedRecord is SparePartData sparePartData)
            {
                await _databaseHelper.UpdateRecordAsync(sparePartData);
            }

            MessageBox.Show("Record updated successfully.");
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error updating record: {ex.Message}");
        }
    }

    private async Task DeleteRecordAsync()
    {
        try
        {
            if(SelectedRecord is SAPData sapData)
            {
                await _databaseHelper.DeleteRecordAsync(sapData);
            }
            else if(SelectedRecord is SparePartData sparePartData)
            {
                await _databaseHelper.DeleteRecordAsync(sparePartData);
            }

            UnifiedData.Remove(SelectedRecord);
            SelectedRecord = null;
            MessageBox.Show("Record deleted successfully.");
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error deleting record: {ex.Message}");
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