using MaintenanceApp.WPF.Helper;
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
    private readonly IDatabaseHelper _databaseHelper;
    public ObservableCollection<SAPData> SAPDataRecords { get; set; } = new();
    public ObservableCollection<VisitData> VisitDataRecords { get; set; } = new();
    public ObservableCollection<SAPData> FilteredSAPDataRecords { get; set; } = new();


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

    private string _newNotification;
    public string NewNotification
    {
        get => _newNotification;
        set
        {
            _newNotification = value;
            OnPropertyChanged(nameof(NewNotification));
        }
    }

    private string _searchKeyword;
    public string SearchKeyword
    {
        get => _searchKeyword;
        set
        {
            _searchKeyword = value;
          ApplyFilter(); 
            OnPropertyChanged(nameof(SearchKeyword));
            
        }
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddSAPDataCommand { get; }
    public ICommand AddVisitDataCommand { get; }
    public ICommand DeleteVisitDataCommand { get; }
    public ICommand UpdateVisitDataCommand { get; }
    public ICommand RowSelectedCommand { get; }

    public AddVisitDataViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;

        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
        AddSAPDataCommand = new RelayCommand(async _ => await AddSAPDataAsync());
        AddVisitDataCommand = new RelayCommand(async _ => await AddVisitDataAsync());
        DeleteVisitDataCommand = new RelayCommand(async _ => await DeleteVisitDataAsync());
        UpdateVisitDataCommand = new RelayCommand(async _ => await UpdateVisitDataAsync());

        RowSelectedCommand = new RelayCommand<object>(OnRowSelected);

        // تحميل البيانات عند بدء التطبيق
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            SAPDataRecords.Clear();
            var sapDataList = await _databaseHelper.GetAllRecordsAsync<SAPData>();
            foreach(var sap in sapDataList)
            {
                SAPDataRecords.Add(sap);
            }
            FilteredSAPDataRecords = new ObservableCollection<SAPData>(SAPDataRecords);
            OnPropertyChanged(nameof(FilteredSAPDataRecords));
     MessageBox.Show($"FilteredSAPDataRecords.Count: {FilteredSAPDataRecords.Count}");
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error loading data: {ex.Message}");
        }
    }


    private void ApplyFilter()
    {
        if(string.IsNullOrWhiteSpace(SearchKeyword))
        {
            FilteredSAPDataRecords = new ObservableCollection<SAPData>(SAPDataRecords);
            MessageBox.Show($"ApplyFilterif FilteredSAPDataRecords.Count: {FilteredSAPDataRecords.Count}");
        }
        else
        {
            FilteredSAPDataRecords = new ObservableCollection<SAPData>(
                SAPDataRecords.Where(s => s.Notification.ToString()?.Contains(SearchKeyword) == true));
            MessageBox.Show($"ApplyFilterelse FilteredSAPDataRecords.Count: {FilteredSAPDataRecords.Count}");
        }
    }


    private async Task AddSAPDataAsync()
    {
        if(string.IsNullOrWhiteSpace(NewNotification)) return;

        try
        {
            var newSAPData = new SAPData
            {
                Notification = Convert.ToDouble(NewNotification) ,
                NotificationType = "New Type" ,
                NotifDate = DateTime.Now
            };

            await _databaseHelper.InsertRecordAsync(newSAPData);
            SAPDataRecords.Add(newSAPData);
            FilteredSAPDataRecords.Add(newSAPData);

            NewNotification = string.Empty; // إعادة تعيين الإدخال
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error adding SAP data: {ex.Message}");
        }
    }

    private void OnRowSelected(object parameter)
    {
        if(parameter is SAPData selectedSapData)
        {
            SelectedSAPData = selectedSapData;
            MessageBox.Show($"تم تحديد البيانات: {selectedSapData.Notification}");
        }
    }

    private void LoadVisitData()
    {
        if(SelectedSAPData != null)
        {
            VisitDataRecords = new ObservableCollection<VisitData>(SelectedSAPData.Visits);
            OnPropertyChanged(nameof(VisitDataRecords));
        }
    }

    private async Task AddVisitDataAsync()
    {
        if(SelectedSAPData == null) return;

        try
        {
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
        catch(Exception ex)
        {
            MessageBox.Show($"Error adding visit data: {ex.Message}");
        }
    }

    private async Task DeleteVisitDataAsync()
    {
        if(SelectedVisitData == null) return;

        try
        {
            await _databaseHelper.DeleteRecordAsync(SelectedVisitData);
            SelectedSAPData.Visits.Remove(SelectedVisitData);
            LoadVisitData();
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error deleting visit data: {ex.Message}");
        }
    }

    private async Task UpdateVisitDataAsync()
    {
        if(SelectedVisitData == null) return;

        try
        {
            SelectedVisitData.Technician = "Updated Technician";
            await _databaseHelper.UpdateRecordAsync(SelectedVisitData);
            LoadVisitData();
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error updating visit data: {ex.Message}");
        }
    }
}