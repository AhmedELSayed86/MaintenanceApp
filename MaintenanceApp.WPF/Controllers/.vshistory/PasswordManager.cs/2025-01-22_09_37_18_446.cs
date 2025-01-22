using MaintenanceApp.WPF.Helper;
using System;
using System.IO;

namespace MaintenanceApp.WPF.Controllers;

public static class PasswordManager
{
    private static readonly string PasswordFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) ,"EncryptedPassword.txt");

    public static void SaveEncryptedPassword(string password)
    {
        string encryptedPassword = EncryptionHelper.Encrypt(password);
        File.WriteAllText(PasswordFilePath ,encryptedPassword);
    }

    public static string LoadEncryptedPassword()
    {
        if(File.Exists(PasswordFilePath))
        {
            string encryptedPassword = File.ReadAllText(PasswordFilePath);
            return EncryptionHelper.Decrypt(encryptedPassword);
        }
        throw new FileNotFoundException("Password file not found.");
    }
}
