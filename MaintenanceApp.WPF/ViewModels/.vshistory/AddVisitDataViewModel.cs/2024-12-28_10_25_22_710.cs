﻿using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels;

public class AddVisitDataViewModel : BaseViewModel
{
    private double _selectedNotification;
    private readonly IDatabaseHelper _databaseHelper;
    public ObservableCollection<SAPData> SAPDataRecords { get; set; } = new();
    public ObservableCollection<VisitData> VisitDataRecords { get; set; } = new();
    public ObservableCollection<dynamic> LinkedRecords { get; set; } = new();

    private SAPData _selectedSAPData;
    public SAPData SelectedSAPData
    {
        get => _selectedSAPData;
        set
        {
            _selectedSAPData = value; 
            LoadVisitData();
            OnPropertyChanged(nameof(SelectedSAPData)); 
        }
    }

    private VisitData _selectedVisitData;
    public VisitData SelectedVisitData
    {
        get => _selectedVisitData;
        set
        {
            _selectedVisitData = value;
            OnPropertyChanged(nameof(SelectedVisitData)); // إعلام واجهة المستخدم بالتغيير
        }
    }

    private ObservableCollection<SAPData> _filteredSAPDataRecords;
    public ObservableCollection<SAPData> FilteredSAPDataRecords
    {
        get => _filteredSAPDataRecords;
        set
        {
            _filteredSAPDataRecords = value;
            OnPropertyChanged(nameof(FilteredSAPDataRecords));
        }
    }

    private string _newCity;
    public string NewCity
    {
        get => _newCity;
        set
        {
            _newCity = value;
            OnPropertyChanged(nameof(NewCity));
        }
    }

    private string _searchKeyword;
    public string SearchKeyword
    {
        get => _searchKeyword;
        set
        {
            _searchKeyword = value;
            OnPropertyChanged(nameof(SearchKeyword));
            ApplyFilter();
        }
    }

    public double SelectedNotification
    {
        get => _selectedNotification;
        set
        {
            _selectedNotification = value;
            LoadVisitData(); // تحميل البيانات عند تغيير الإشعار المحدد
            OnPropertyChanged(nameof(SelectedNotification)); 
        }
    }

    private const int DefaultPageSize = 20;
    private int _currentPage = 1;
    private bool _hasMoreData = true;

    public ICommand LoadDataCommand { get; }
    public ICommand AddSAPDataCommand { get; }
    public ICommand AddVisitDataCommand { get; }
    public ICommand DeleteVisitDataCommand { get; }
    public ICommand UpdateVisitDataCommand { get; }
    public ICommand LoadMoreDataCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public ICommand LoadLinkedDataCommand { get; }
    public ICommand RowSelectedCommand { get; set; }

    public AddVisitDataViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;

        LoadDataCommand = new RelayCommand(async o => await LoadDataAsync());
        AddSAPDataCommand = new RelayCommand(async o => await AddSAPDataAsync());
        AddVisitDataCommand = new RelayCommand(async o => await AddVisitDataAsync());
        DeleteVisitDataCommand = new RelayCommand(async o => await DeleteVisitDataAsync());
        UpdateVisitDataCommand = new RelayCommand(async o => await UpdateVisitDataAsync());

        // أوامر مفقودة
        LoadMoreDataCommand = new RelayCommand(async o => await LoadMoreDataAsync());
        ApplyFilterCommand = new RelayCommand(async o => await ApplyFilterAsync());
        LoadLinkedDataCommand = new RelayCommand(async o => await LoadLinkedDataAsync());

 FilteredSAPDataRecords = new ObservableCollection<SAPData>();
        RowSelectedCommand = new RelayCommand(OnRowSelected);
       

        LoadDataAsync1();
    }

    private async void LoadDataAsync1()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            SAPDataRecords.Clear();
            var sapDataList = await _databaseHelper.GetAllRecordsAsync<SAPData>();
            if(!sapDataList.Any())
            {
                MessageBox.Show("No data returned from SAPDatas!");
                return;
            }
            foreach(var sap in sapDataList)
            {
                var visits = await _databaseHelper.GetFilteredRecordsAsync<VisitData>("VisitDatas" ,"Notification" ,sap.Notification.ToString());
                sap.Visits = new ObservableCollection<VisitData>(visits);
                SAPDataRecords.Add(sap);
            } 
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }

    private void OnRowSelected(object parameter)
    {
        if(parameter is SAPData selectedSapData)
        {
            SelectedNotification = selectedSapData.Notification;
        }
    }

    private async Task LoadMoreDataAsync()
    {
        if(!_hasMoreData) return;

        var sapDataList = await _databaseHelper.GetPagedRecordsAsync<SAPData>("SAPDatas" ,_currentPage ,DefaultPageSize);
        if(sapDataList.Count < DefaultPageSize) _hasMoreData = false;

        foreach(var sap in sapDataList)
        {
            SAPDataRecords.Add(sap);
        }

        _currentPage++;
    }

    private void ApplyFilter()
    {
        if(string.IsNullOrWhiteSpace(SearchKeyword))
        {
            FilteredSAPDataRecords = new ObservableCollection<SAPData>(SAPDataRecords);
        }
        else
        {
            FilteredSAPDataRecords = new ObservableCollection<SAPData>(
                SAPDataRecords.Where(s => s.Notification.ToString().Contains(SearchKeyword)));
        }
        OnPropertyChanged(nameof(FilteredSAPDataRecords));
    }

    private async Task ApplyFilterAsync()
    {
        SAPDataRecords.Clear();
        var filteredData = await _databaseHelper.GetFilteredRecordsAsync<SAPData>("SAPDatas" ,"City" ,"Cairo");
        foreach(var sap in filteredData)
        {
            SAPDataRecords.Add(sap);
        }
    }

    private async Task LoadLinkedDataAsync()
    {
        try
        {
            LinkedRecords.Clear();
            var linkedData = await _databaseHelper.GetLinkedRecordsAsync();
            if(linkedData == null || linkedData.Count() == 0)
            { 
                return;
            }

            foreach(var record in linkedData)
            {
                LinkedRecords.Add(record);
            } 
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error DatabaseHelper.GetLinkedRecordsAsync: {ex.Message}");
        }
    }

    private void LoadVisitData()
    {
        if(SelectedSAPData != null)
        {
            VisitDataRecords = new ObservableCollection<VisitData>(
                SelectedSAPData.Visits.Select(v =>
                {
                    v.Notification = SelectedNotification;
                    return v;
                })
            );
            OnPropertyChanged(nameof(VisitDataRecords));
        }
    }

    private async Task AddSAPDataAsync()
    {
        if(string.IsNullOrWhiteSpace(NewCity)) return;

        var newSAPData = new SAPData
        {
            Notification = new Random().NextDouble() ,
            NotificationType = "New Type" ,
            NotifDate = DateTime.Now ,
            City = NewCity
        };

        await _databaseHelper.InsertRecordAsync(newSAPData);
        SAPDataRecords.Add(newSAPData);
        FilteredSAPDataRecords.Add(newSAPData); // تحديث الفلتر
        NewCity = string.Empty; // إعادة تعيين الإدخال
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