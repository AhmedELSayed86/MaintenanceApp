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

namespace MaintenanceApp.WPF.ViewModels;

public class UsersViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;

    public ObservableCollection<User> _users { get; set; } = new();
    public ICommand AddUserCommand { get; }
    public ICommand UpdateUserCommand { get; }
    public ICommand DeleteUserCommand { get; }

    private User _selectedUser;
    public User SelectedUser
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
            var userList = await _databaseHelper.GetAllRecordsAsync<User>();
            _users = new ObservableCollection<User>(userList);
            OnPropertyChanged(nameof(_users));
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
            SelectedUser.PasswordHash = HashPassword(NewPassword);
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
                SelectedUser.PasswordHash = HashPassword(NewPassword);

            await _databaseHelper.UpdateRecordAsync(SelectedUser);
            OnPropertyChanged(nameof(_users));
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error updating user: {ex.Message}");
        }
    }

    private async Task DeleteUserAsync()
    {
        if(SelectedUser == null)
        {
            MessageBox.Show("No user selected for deletion.");
            return;
        }

        try
        {
            await _databaseHelper.DeleteRecordAsync(SelectedUser);
            _users.Remove(SelectedUser);
            SelectedUser = null;
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error deleting user: {ex.Message}");
        }
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
