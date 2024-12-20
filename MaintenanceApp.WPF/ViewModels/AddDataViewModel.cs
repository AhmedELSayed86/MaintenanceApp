using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels
{
    public class AddDataViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private VisitData _newRecord = new VisitData();
        public VisitData NewRecord
        {
            get => _newRecord;
            set { _newRecord = value; OnPropertyChanged(nameof(NewRecord)); }
        }

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; FilteredRecords.Refresh(); }
        }

        private ObservableCollection<VisitData> _records = new ObservableCollection<VisitData>();
        public ObservableCollection<VisitData> Records
        {
            get => _records;
            set
            {
                _records = value; OnPropertyChanged(nameof(Records));
                OnPropertyChanged(nameof(HasData)); // تحديث حالة البيانات
            }
        }

        // خاصية لمعرفة وجود بيانات
        public bool HasData => Records != null && Records.Any();

        public ICollectionView FilteredRecords { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddDataViewModel(Action onSave ,Action onCancel)
        {  // جلب البيانات من قاعدة البيانات عند تهيئة ViewModel
           //   Records = new ObservableCollection<VisitData>(DatabaseHelper.GetAllRecords());
            FilteredRecords = CollectionViewSource.GetDefaultView(Records);
            FilteredRecords.Filter = FilterRecords;
            SaveCommand = new RelayCommand(o => SaveNewRecord() ,o => CanSave());
            CancelCommand = new RelayCommand(o => onCancel());
        }

        private bool FilterRecords(object item)
        {
            if(item is VisitData record)
                return string.IsNullOrEmpty(SearchQuery) ||
                       record.Technician?.IndexOf(SearchQuery ,StringComparison.OrdinalIgnoreCase) >= 0;
            return false;
        }

        private void SaveNewRecord()
        {
            // إضافة السجل الجديد إلى قاعدة البيانات
            //   DatabaseHelper.SaveRecord(NewRecord);
            Records.Add(NewRecord);  // إضافة السجل إلى القائمة
            NewRecord = new VisitData();  // إعادة تعيين السجل الجديد
        }

        // الأوامر       
        public ICommand DeleteCommand { get; }

        private void DeleteRecord(object parameter)
        {
            if(parameter is VisitData record)
                Records.Remove(record);
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(NewRecord?.Technician);

        // Notify Changes
        private void OnPropertyChanged(string propertyName) =>
         PropertyChanged?.Invoke(this ,new PropertyChangedEventArgs(propertyName));
    }
}
