using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MaintenanceApp.WPF.Helper;

public static class EncryptionHelper
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("YourEncryptionKey123"); // مفتاح التشفير (يجب أن يكون 16/24/32 بايت)
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("YourInitializationVector"); // IV (يجب أن يكون 16 بايت)

    public static byte[] Encrypt(string plainText)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Key;
        aesAlg.IV = IV;

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key ,aesAlg.IV);

        using MemoryStream msEncrypt = new();
        using CryptoStream csEncrypt = new(msEncrypt ,encryptor ,CryptoStreamMode.Write);
        using(StreamWriter swEncrypt = new(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }
        return msEncrypt.ToArray();
    }

    public static string Decrypt(byte[] cipherText)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Key;
        aesAlg.IV = IV;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key ,aesAlg.IV);

        using MemoryStream msDecrypt = new(cipherText);
        using CryptoStream csDecrypt = new(msDecrypt ,decryptor ,CryptoStreamMode.Read);
        using StreamReader srDecrypt = new(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
}
