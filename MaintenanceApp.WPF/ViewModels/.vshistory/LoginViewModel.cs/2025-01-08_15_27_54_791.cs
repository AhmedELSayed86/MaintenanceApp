using MaintenanceApp.WPF.Helper;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;
using System.Linq;
using MaintenanceApp.WPF.Controllers;

namespace MaintenanceApp.WPF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IDatabaseHelper _databaseHelper;
        public ICommand LoginCommand { get; }
        public ICommand ClosedCommand { get; }
         
        private string _username="AHMED";
        public string Username 
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _userPassword="123456";
        public string UserPassword
        {
            get => _userPassword;
            set
            {
                _userPassword = value;
                OnPropertyChanged(nameof(UserPassword));
            }
        }

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

        public LoginViewModel(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));

            LoginCommand = new RelayCommand(async _ => await LoginAsync());
            ClosedCommand = new RelayCommand(_ => ClosedWindow());
        }

        private void ClosedWindow()
        {
            LoginWindow mainViewModel = System.Windows.Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault() ?? new LoginWindow();
            mainViewModel.Close();
        }

        private async Task LoginAsync()
        {
            try
            {
                if(string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(UserPassword))
                {
                    Summary = "يجب إدخال اسم المستخدم وكلمة المرور";
                    return;
                }

                var user = await _databaseHelper.GetUserByUsernameAsync(Username);

                if(user != null && HashHelper.VerifyPassword(UserPassword ,user.PasswordHash))
                {
                    CurrentUser.Instance.SetUser(user.ID ,$"مرحبا: {user.UserName}" );

                    Summary = "تم تسجيل الدخول بنجاح";
                    // Navigate to the main page or dashboard
                    ClosedWindow();
                }
                else
                {
                    Summary = "اسم المستخدم أو كلمة المرور غير صالحة";
                }
            }
            catch(Exception ex)
            {
                Summary = $"حدث خطأ أثناء تسجيل الدخول: {ex.Message}";
            }
            finally
            {
                UserPassword = string.Empty;
            }
        }

    }
}
