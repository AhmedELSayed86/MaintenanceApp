using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels;

public class AddDataViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;
    public ObservableCollection<SAPData> Records { get; set; } = [];

    private SAPData _newRecord = new();
    public SAPData NewRecord
    {
        get => _newRecord;
        set
        {
            _newRecord = value;
            OnPropertyChanged();
        }
    }

    private string _searchQuery;
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            OnPropertyChanged();
        }
    }

    public ICollectionView FilteredRecords { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public AddDataViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;

        FilteredRecords = CollectionViewSource.GetDefaultView(Records);
        FilteredRecords.Filter = FilterRecords;

        SaveCommand = new RelayCommand(o => SaveNewRecord());
        CancelCommand = new RelayCommand(o => CancelOperation());
    }

    private bool FilterRecords(object item)
    {
        if(item is SAPData record)
            return string.IsNullOrEmpty(SearchQuery) ||
                   record.Technician?.IndexOf(SearchQuery ,StringComparison.OrdinalIgnoreCase) >= 0;
        return false;
    }

    private async void SaveNewRecord()
    {
        try
        {
            await _databaseHelper.InsertRecordAsync(NewRecord);
            Records.Add(NewRecord);
            NewRecord = new SAPData();
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error saving record: {ex.Message}");
        }
    }

    private bool CanSave() => !string.IsNullOrWhiteSpace(NewRecord.Notification.ToString());

    private void CancelOperation() => NewRecord = new SAPData();









    // الأوامر       
    public ICommand DeleteCommand { get; }

    private void DeleteRecord(object parameter)
    {
        if(parameter is SAPData record)
            Records.Remove(record);
    }


}
