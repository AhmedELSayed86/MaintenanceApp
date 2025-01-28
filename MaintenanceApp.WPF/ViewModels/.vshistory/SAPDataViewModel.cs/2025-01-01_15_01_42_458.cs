using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels;

public class SAPDataViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;

    public ObservableCollection<SAPData> SAPDatas { get; set; }

    private SAPData _selectedSAPData;
    public SAPData SelectedSAPData
    {
        get => _selectedSAPData;
        set
        {
            _selectedSAPData = value;
            OnPropertyChanged();
        }
    }

    private string _summary;
    public string Summary
    {
        get => _summary;
        set
        {
            _summary = value;
            OnPropertyChanged(nameof(Summary));
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    private bool _isLoading = false;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    public SAPDataViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;

        SAPDatas = new ObservableCollection<SAPData>();
    }

    // التفاف استدعاء المهام غير المتزامنة
    private async void ExecuteLoadData()
    {
        await LoadDataAsync();
    }

    private async void ExecuteSaveData()
    {
        await SaveDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var data = await _databaseHelper.GetAllRecordsAsync<SAPData>();
        SAPDatas.Clear();
        foreach(var item in data)
            SAPDatas.Add(item);
    }

    private async Task SaveDataAsync()
    {
        if(SelectedSAPData != null && ValidateData())
        {
            await _databaseHelper.UpdateRecordAsync(SelectedSAPData);
        }
    }

    private bool ValidateData()
    {
        ClearErrors(nameof(SelectedSAPData.Notification));

        if(string.IsNullOrWhiteSpace(SelectedSAPData.Notification.ToString()))
        {
            AddError(nameof(SelectedSAPData.Notification) ,"Notification cannot be empty.");
            return false;
        }
        return true;
    }
}
