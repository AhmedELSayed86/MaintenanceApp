namespace MaintenanceApp.WPF.Helper;

public static class HashHelper
{
    // تجزئة كلمة المرور باستخدام BCrypt
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // التحقق من صحة كلمة المرور
    public static bool VerifyPassword(string password ,string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password ,hashedPassword);
    }
}

