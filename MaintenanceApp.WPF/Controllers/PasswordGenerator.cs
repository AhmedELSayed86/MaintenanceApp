using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceApp.WPF.Controllers;

public static class PasswordGenerator
{
    private static readonly string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";

    public static string GeneratePassword(int length = 12)
    {
        var random = new Random();
        var password = new StringBuilder();

        for(int i = 0; i < length; i++)
        {
            int index = random.Next(AllowedChars.Length);
            password.Append(AllowedChars[index]);
        }

        return password.ToString();
    }
}
