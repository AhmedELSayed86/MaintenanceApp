using MaintenanceApp.WPF.ViewModels;
using System;

namespace MaintenanceApp.WPF.Helper;

public class CurrentUser : BaseViewModel
{ 
    private static readonly Lazy<CurrentUser> _instance = new Lazy<CurrentUser>(() => new CurrentUser());

    public static CurrentUser Instance => _instance.Value;
    
    private int _userID;
    public int UserID
    {
        get => _userID;
        set
        {
            _userID = value;
            OnPropertyChanged(nameof(UserID));
        }
    }
    
    private string _username;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }
    private CurrentUser() { }

    public void SetUser(int userId ,string username)
    {
        UserID = userId;
        Username = username;
    }

    public void ClearUser()
    {
        UserID = 0;
        Username = string.Empty;
    }
}
