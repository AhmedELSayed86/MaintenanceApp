﻿using MaintenanceApp.WPF.Helper;
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
        public ICommand ClosedCommand { get; }

        public string Username { get; set; }
        public string Password { get; set; }

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
                if(Username.Length > 0 && Password.Length > 0)
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
            }
            finally
            {
                Password = string.Empty;
            }
        }
    }
}
