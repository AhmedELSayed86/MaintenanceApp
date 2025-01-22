using MaintenanceApp.WPF.ViewModels;
using System;
using System.ComponentModel;

namespace MaintenanceApp.WPF.Helper;

public class CurrentUser : BaseViewModel
{
    private static readonly Lazy<CurrentUser> _instance = new(() => new CurrentUser());
    public static CurrentUser Instance => _instance.Value;

    private int _userId;
    public int UserId
    {
        get => _userId;
        private set
        {
            _userId = value;
            OnPropertyChanged(nameof(UserId));
        }
    }

    private string _username;
    public string Username
    {
        get => _username;
        private set
        { 
                _username = value; 
            OnPropertyChanged(nameof(Username));
        }
    }

    private CurrentUser() { }

    public void SetUser(int userId ,string username)
    {
        UserId = userId;
        Username = username;
    }

    public void ClearUser()
    {
        UserId = 0;
        Username = string.Empty;
    }
}

