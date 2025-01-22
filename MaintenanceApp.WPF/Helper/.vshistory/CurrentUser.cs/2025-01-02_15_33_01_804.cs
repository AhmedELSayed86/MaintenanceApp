using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceApp.WPF.Helper;

public class CurrentUser
{
    private static CurrentUser _instance;
    public static CurrentUser Instance => _instance ??= new CurrentUser();

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
