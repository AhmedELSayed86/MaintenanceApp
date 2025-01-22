using MaintenanceApp.WPF.Helper;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace MaintenanceApp.WPF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly DatabaseHelper _dbHelper;
        public ICommand LoginCommand { get; }

        public string Username { get; set; }
        public string Password { get; set; }

        public LoginViewModel()
        {
            _dbHelper = new DatabaseHelper();
            LoginCommand = new RelayCommand(async _ => await LoginAsync());
        }

        private async Task LoginAsync()
        {
            try
            {
                var user = await _dbHelper.GetUserAsync(Username);

                if(user != null && HashHelper.VerifyPassword(Password ,user.HashedPassword))
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
                // تأكد من تنظيف كلمة المرور من الذاكرة
                Password = string.Empty;
            }
        }
    }
}
