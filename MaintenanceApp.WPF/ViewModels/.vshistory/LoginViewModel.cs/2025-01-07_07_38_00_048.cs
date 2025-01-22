using MaintenanceApp.WPF.Helper;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;
using System.Linq;

namespace MaintenanceApp.WPF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IDatabaseHelper _databaseHelper;
        public ICommand LoginCommand { get; }
        public ICommand OpenHomeCommand { get; }

        public string Username { get; set; }
        public string Password { get; set; }

        public LoginViewModel(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
           
            LoginCommand = new RelayCommand(async _ => await LoginAsync());
            OpenHomeCommand=new RelayCommand(async async_=>await OpenHomeWindow()) }

        private async Task OpenHomeWindow()
        {
            MainViewModel mainViewModel = System.Windows.Application.Current.Windows.OfType<MainViewModel>().FirstOrDefault() ?? new MainViewModel();
            mainViewModel.OpenHomeCommand.Execute(this);
        }

        private async Task LoginAsync()
        {
            try
            {
                var hashedPassword = HashHelper.HashPassword(Password);
                var user = await _databaseHelper.GetUserAsync(Username ,hashedPassword);

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
