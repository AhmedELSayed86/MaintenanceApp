using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System;
using System.Threading.Tasks;

namespace MaintenanceApp.WPF.ViewModels;

public class AddUserViewModel
{
    private readonly DatabaseHelper _dbHelper;

    public string Username { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    public async Task<bool> AddUserAsync()
    {
        if(Password != ConfirmPassword)
            throw new Exception("Passwords do not match!");

        var hashedPassword = HashPassword(Password);
        var newUser = new User
        {
            UserName = Username ,
            PasswordHash = hashedPassword
        };

        await _dbHelper.InsertRecordAsync(newUser);
        return true;
    } 
}
