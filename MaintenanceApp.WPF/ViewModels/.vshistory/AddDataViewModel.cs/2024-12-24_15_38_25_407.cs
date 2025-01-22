using MaintenanceApp.WPF.Helper;
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

public class AddDataViewModel : BaseViewModel
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
        }
    }

    public ICollectionView FilteredRecords { get; }

    // الأوامر
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand LoadDataCommand { get; }

    public AddDataViewModel(IDatabaseHelper databaseHelper)
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

    private bool FilterRecords(object item)
    {
        if(item is SAPData record)
            return string.IsNullOrEmpty(SearchQuery) ||
                   record.Notification.ToString().IndexOf(SearchQuery ) >= 0;
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
