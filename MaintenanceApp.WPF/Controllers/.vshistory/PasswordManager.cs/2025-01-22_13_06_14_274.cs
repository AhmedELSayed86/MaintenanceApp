using MaintenanceApp.WPF.Helper;
using System;
using System.IO;

namespace MaintenanceApp.WPF.Controllers;

public static class PasswordManager
{
    private static readonly string PasswordFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) ,"encryptedPassword.bin");

    public static void SaveEncryptedPassword(string password)
    {
        byte[] encryptedPassword = EncryptionHelper.Encrypt(password);
        File.WriteAllBytes(PasswordFilePath ,encryptedPassword);
    }

    public static string LoadEncryptedPassword()
    {
        if(File.Exists(PasswordFilePath))
        {
            byte[] encryptedData = File.ReadAllBytes(PasswordFilePath);
            return EncryptionHelper.Decrypt(encryptedData);
        }
        throw new FileNotFoundException("Password file not found.");
    }
}
