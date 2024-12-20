using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MaintenanceApp.WPF.ViewModels;

public class SAPDataViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;

    public ObservableCollection<SAPData> SAPDatas { get; set; }

    public DelegateCommand LoadCommand { get; }
    public DelegateCommand SaveCommand { get; }

    private SAPData _selectedSAPData;
    public SAPData SelectedSAPData
    {
        get => _selectedSAPData;
        set => SetProperty(ref _selectedSAPData ,value);
    }

    public SAPDataViewModel(IDatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;

        SAPDatas = new ObservableCollection<SAPData>();
        LoadCommand = new DelegateCommand(ExecuteLoadData);
        SaveCommand = new DelegateCommand(ExecuteSaveData);
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