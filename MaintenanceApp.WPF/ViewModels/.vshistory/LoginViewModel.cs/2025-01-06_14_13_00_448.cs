using MaintenanceApp.WPF.Helper;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace MaintenanceApp.WPF.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly DatabaseHelper _dbHelper;
    public ICommand LoginCommand { get; }

    public string Username { get; set; }
    public string Password { get; set; }

    public LoginViewModel()
    {
        _dbHelper = new DatabaseHelper();
        LoginCommand = new RelayCommand(_ => Login());
    }

    private async void Login()
    {
        var hashedPassword = HashHelper.HashPassword(Password);
        var user = await _dbHelper.GetUserAsync(Username ,hashedPassword);

        if(user != null)
        {
            CurrentUser.Instance.SetUser(user.ID ,user.UserName);
            MessageBox.Show("Login successful!");
            // Navigate to the main page or dashboard
        }
        else
        {
            MessageBox.Show("Invalid username or password.");
        }
    }

    public async Task<bool> LoginAsync()
    {
        var hashedPassword = HashPassword(Password);
        var user = await _dbHelper.GetUserAsync(Username ,hashedPassword);

        if(user != null)
        {
            CurrentUser.Instance.SetUser(user.ID ,user.UserName);
            MessageBox.Show("Login successful!");
            // Navigate to the main page or dashboard
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
