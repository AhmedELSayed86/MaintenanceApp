using System;

namespace MaintenanceApp.WPF.Helper;

public class CurrentUser
{ 
    private static readonly Lazy<CurrentUser> _instance = new Lazy<CurrentUser>(() => new CurrentUser());

    public static CurrentUser Instance => _instance.Value;
    public int UserId { get; private set; }
    public string Username { get; private set; }

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
