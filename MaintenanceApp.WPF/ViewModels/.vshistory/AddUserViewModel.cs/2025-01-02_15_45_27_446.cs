using DocumentFormat.OpenXml.Math;
using MaintenanceApp.WPF.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Username = Username ,
            PasswordHash = hashedPassword
        };

        await _dbHelper.AddUserAsync(newUser);
        return true;
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}

