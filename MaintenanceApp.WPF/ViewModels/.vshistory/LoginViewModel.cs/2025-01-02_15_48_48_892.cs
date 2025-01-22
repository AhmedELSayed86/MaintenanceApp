using MaintenanceApp.WPF.Helper;
using System.Threading.Tasks;

namespace MaintenanceApp.WPF.ViewModels;

public class LoginViewModel
{
    private readonly DatabaseHelper _dbHelper;

    public string Username { get; set; }
    public string Password { get; set; }

    public async Task<bool> LoginAsync()
    {
        var hashedPassword = HashPassword(Password);
        var user = await _dbHelper.GetUserAsync(Username ,hashedPassword);

        if(user != null)
        {
            CurrentUser.Instance.SetUser(user.UserId ,user.Username);
            return true;
        }

        return false;
    }

    private string HashPassword(string password)
    {
        // Implement hashing logic here
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
