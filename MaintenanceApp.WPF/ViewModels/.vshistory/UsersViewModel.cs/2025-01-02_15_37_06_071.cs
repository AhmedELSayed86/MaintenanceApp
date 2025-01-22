using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        private readonly IDatabaseHelper _databaseHelper;

        public ObservableCollection<Users> _users { get; set; } = new();
        public ICommand AddUserCommand { get; }
        public ICommand UpdateUserCommand { get; }
        public ICommand DeleteUserCommand { get; }

        private Users _selectedUser;
        public Users SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
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

        public UsersViewModel(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));

            AddUserCommand = new RelayCommand(async _ => await AddUserAsync());
            UpdateUserCommand = new RelayCommand(async _ => await UpdateUserAsync());
            DeleteUserCommand = new RelayCommand(async _ => await DeleteUserAsync());

            _ = LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var userList = await _databaseHelper.GetAllRecordsAsync<Users>();
                _users = new ObservableCollection<Users>(userList);
                OnPropertyChanged(nameof(Users));
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}");
            }
        }

        private async Task AddUserAsync()
        {
            if(SelectedUser == null || string.IsNullOrWhiteSpace(NewPassword))
            {
                MessageBox.Show("Please provide valid user details.");
                return;
            }

            try
            {
                SelectedUser.PasswordHash = HashPasswordPBKDF2(NewPassword);
                SelectedUser.CreatedOn = DateTime.Now;
                SelectedUser.ChangeOn = DateTime.Now;

                await _databaseHelper.InsertRecordAsync(SelectedUser);
                _users.Add(SelectedUser);

                SelectedUser = null;
                NewPassword = string.Empty;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error adding user: {ex.Message}");
            }
        }

        private async Task UpdateUserAsync()
        {
            if(SelectedUser == null)
            {
                MessageBox.Show("No user selected for update.");
                return;
            }

            try
            {
                SelectedUser.ChangeOn = DateTime.Now;

                if(!string.IsNullOrWhiteSpace(NewPassword))
                    SelectedUser.PasswordHash = HashPasswordPBKDF2(NewPassword);

                await _databaseHelper.UpdateRecordAsync(SelectedUser);
                OnPropertyChanged(nameof(Users));
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error updating user: {ex.Message}");
            }
        }

        /// <summary>
        /// لا يتم حذف المستخدم ولكن تحويله من نشط الى غير نشط
        /// </summary>
        /// <returns></returns>
        private async Task DeleteUserAsync()
        {
            if(SelectedUser == null)
            {
                MessageBox.Show("No user selected for update.");
                return;
            }

            try
            {
                SelectedUser.ChangeOn = DateTime.Now;

                if(!string.IsNullOrWhiteSpace(NewPassword))
                    SelectedUser.PasswordHash = HashPasswordPBKDF2(NewPassword);

                await _databaseHelper.UpdateRecordAsync(SelectedUser);
                OnPropertyChanged(nameof(Users));
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error updating user: {ex.Message}");
            }
        }

        private string HashPasswordPBKDF2(string password)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password ,16 ,10000);
            var salt = deriveBytes.Salt;
            var hash = deriveBytes.GetBytes(32);

            var saltHash = new byte[48];
            Array.Copy(salt ,0 ,saltHash ,0 ,16);
            Array.Copy(hash ,0 ,saltHash ,16 ,32);

            return Convert.ToBase64String(saltHash);
        }
    }
}
