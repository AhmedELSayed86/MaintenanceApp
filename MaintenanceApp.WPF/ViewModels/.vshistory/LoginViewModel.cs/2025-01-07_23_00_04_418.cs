﻿using MaintenanceApp.WPF.Helper;
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

        public string Username { get; set; }
      

        private string _userPassword;
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
                if(string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(UserPassword))
                {
                    Summary = "يجب ادخال اسم المستخدم و كلمة المرور"  ;
                    return;
                }
                var hashedPassword = HashHelper.HashPassword(UserPassword);
                var user = await _databaseHelper.GetUserAsync(Username ,hashedPassword);

                if(user != null && HashHelper.VerifyPassword(UserPassword ,user.PasswordHash))
                {
                    CurrentUser.Instance.SetUser(user.ID ,user.UserName);
                    
                    Summary = "تم تسجيل الدخول بنجاح";
                    // Navigate to the main page or dashboard
                }
                else
                {
                    Summary = "اسم المستخدم أو كلمة المرور غير صالحة"; 
                }
            }
            finally
            {
                UserPassword = string.Empty;
            }
        }
    }
}
