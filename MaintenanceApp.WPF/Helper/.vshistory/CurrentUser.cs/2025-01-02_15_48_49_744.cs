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

    public static int UserId { get; private set; }
    public static string Username { get; private set; }

    private CurrentUser() { }

    public static void SetUser(int userId ,string username)
    {
        UserId = userId;
        Username = username;
    }

    public static void ClearUser()
    {
        UserId = 0;
        Username = string.Empty;
    } 
}
