using MaintenanceApp.WPF.Controllers;
using MaintenanceApp.WPF.Helper;
using MaintenanceApp.WPF.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels;

public class AddUserViewModel : BaseViewModel
{
    private readonly IDatabaseHelper _databaseHelper;

    public string Username { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    private string _summary;
    public string Summary
    {
        get => _summary;
        set
        {
            _summary = value;
            OnPropertyChanged(nameof(Summary));
            MessageController.SummaryAsync(Summary);
            CommandManager.InvalidateRequerySuggested(); // تحديث الأوامر
        }
    }

    public async Task<bool> AddUserAsync()
    {
        if(Password != ConfirmPassword)
            Summary = "Passwords do not match!";

        var hashedPassword = HashHelper.HashPassword(Password);
        var newUser = new User
        {
            UserName = Username ,
            PasswordHash = hashedPassword
        };

        await _databaseHelper.InsertRecordAsync(newUser);
        Summary = $"تم اضافة المستخدم ({Username}) بنجاح";
        return true;
    }
}
