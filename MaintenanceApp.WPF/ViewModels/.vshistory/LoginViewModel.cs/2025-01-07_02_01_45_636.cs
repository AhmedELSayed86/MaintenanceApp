using MaintenanceApp.WPF.Helper;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;

namespace MaintenanceApp.WPF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IDatabaseHelper _databaseHelper;
        public ICommand LoginCommand { get; }

        public string Username { get; set; }
        public string Password { get; set; }

        public LoginViewModel(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
           
            LoginCommand = new RelayCommand(async _ => await LoginAsync());
        }

        private async Task LoginAsync()
        {
            try
            {
                var hashedPassword = HashHelper.HashPassword(Password);
                var user = await _dbHelper.GetUserAsync(Username ,hashedPassword);

                if(user != null && HashHelper.VerifyPassword(Password ,user.PasswordHash))
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
            finally
            {
                Password = string.Empty;
            }
        }
    }
}
