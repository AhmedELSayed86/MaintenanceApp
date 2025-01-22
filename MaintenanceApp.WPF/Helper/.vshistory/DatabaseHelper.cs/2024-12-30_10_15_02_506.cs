using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using System.Linq;
using System.Data.Entity;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Windows;
using DocumentFormat.OpenXml.Spreadsheet;

namespace MaintenanceApp.WPF.Helper;

public class DatabaseHelper : IDatabaseHelper
{
    private readonly string _connectionString;

    private static readonly string ResourcesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
    private static readonly string ResourcesPath1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
    private static readonly string dbFilePath = Path.Combine(ResourcesPath ,"MaintenanceApp.db3");
    private static readonly string backupPath = Path.Combine(ResourcesPath1 ,"MaintenanceAppBackup.db3");
    public DatabaseHelper()
    {
        _connectionString = $"Data Source={dbFilePath};Version=3;";
        CreateTables();
    }

    private SQLiteConnection CreateConnection() => new SQLiteConnection(_connectionString);

    private void CreateTables()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        // بيانات العملاء من الساب
        string createSAPDatasTable = @"CREATE TABLE IF NOT EXISTS SAPDatas (  Notification DOUBLE PRIMARY KEY, NotificationType TEXT, Region INTEGER, City TEXT, Street TEXT, ListName TEXT, Telephone TEXT, District TEXT, NotifDate TEXT, Description TEXT, Customer INTEGER, MainWorkCtr TEXT, SortField TEXT, BreakdownDuration INTEGER, RequiredEnd TEXT, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER,  ChangeBy INTEGER, Notes TEXT );";
        // تتميم الفنيين لبلاغات الساب
        string createVisitDatasTable = @"CREATE TABLE IF NOT EXISTS VisitDatas ( ID INTEGER PRIMARY KEY, Notification DOUBLE,  VisitDate TEXT, Technician INTEGER, ServiceDetails TEXT, Implemented TEXT,  Paid TEXT, Unpaid TEXT, Cost REAL, PaymentRefused INTEGER, DistributionStatus TEXT, AssistantTechnician INTEGER, DeterminationTechnician INTEGER, InstallationDate TEXT, ExecutionDuration INTEGER, WarrantyStatus TEXT, UserStatus TEXT,  DiscountPercentage REAL,  CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT);";
        // بيانات قطع الغيار
        string createSparePartDatasTable = @"CREATE TABLE IF NOT EXISTS SparePartDatas ( SapCode INTEGER PRIMARY KEY AUTOINCREMENT, PartNo TEXT, Group_ TEXT, Model TEXT, DescriptionAR TEXT, DescriptionEN TEXT, C1 TEXT, C2 TEXT, IsDamaged BOOLEAN, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT );";
        // عهدة الفني
        string createSparePartsAtTechnicianDatasTable = @"CREATE TABLE IF NOT EXISTS SparePartsAtTechnicianDatas ( ID INTEGER PRIMARY KEY, SapCode INTEGER, TechnicianCode INTEGER, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT );";
        // قطع الغيار التي تم تركيبها على بلاغ معين
        string createSparePartsInstalledDatasTable = @"CREATE TABLE IF NOT EXISTS SparePartsInstalledDatas (  ID INTEGER PRIMARY KEY, Notification DOUBLE, SapCode INTEGER, TechnicianCode INTEGER, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT);";
        // بيانات الموظفين والفنيين
        string createEmployeeDatasTable = @"CREATE TABLE IF NOT EXISTS EmployeeDatas ( Code INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NULL, Job TEXT NULL,  Branch TEXT NULL, WorkLocation TEXT NULL, Department TEXT NULL, Vendor INTEGER NULL,  CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT );";
        // جدول ملاحظات للتذكير
        string createMyNotesDatasTable = @"CREATE TABLE IF NOT EXISTS MyNotesDatas (ID INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT,              Content TEXT, AlertTime DATETIME, Alerted INTEGER, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER,  ChangeBy INTEGER, Notes TEXT );";

        ExecuteNonQuery(connection ,createSAPDatasTable);
        ExecuteNonQuery(connection ,createVisitDatasTable);
        ExecuteNonQuery(connection ,createSparePartsInstalledDatasTable);
        ExecuteNonQuery(connection ,createEmployeeDatasTable);
        ExecuteNonQuery(connection ,createSparePartDatasTable);
        ExecuteNonQuery(connection ,createMyNotesDatasTable);
        ExecuteNonQuery(connection ,createSparePartsAtTechnicianDatasTable);
    }

    private void ExecuteNonQuery(SQLiteConnection connection ,string commandText)
    {
        using var command = new SQLiteCommand(commandText ,connection);
        command.ExecuteNonQuery();
    }

    public async Task<bool> CheckIfSapCodeExistsAsync(string tableName ,double sapCode)
    {
        using var connection = CreateConnection();
        connection.Open();

        var query = $"SELECT COUNT(1) FROM {tableName} WHERE Notification = @sapCode";
        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@sapCode" ,sapCode);

        var result = await Task.Run(() => Convert.ToInt32(command.ExecuteScalar()));
        return result > 0;
    }

    public async Task InsertIntoTableAsync(string tableName ,Dictionary<string ,object> parameters)
    {
        using var connection = CreateConnection();
        connection.Open();

        var columns = string.Join(", " ,parameters.Keys);
        var values = string.Join(", " ,parameters.Keys.Select(k => $"@{k}"));
        var query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

        using var command = connection.CreateCommand();
        command.CommandText = query;

        foreach(var parameter in parameters)
        {
            command.Parameters.AddWithValue($"@{parameter.Key}" ,parameter.Value);
        }

        await Task.Run(() => command.ExecuteNonQuery());
    }

    public async Task<List<T>> GetAllRecordsAsync<T>() where T : class
    {
        try
        {
            return await Task.Run(() =>
            {
                using var connection = CreateConnection();
                connection.Open();
                return connection.GetAll<T>().AsList();
            });
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error GetAllRecordsAsync: {ex.Message}");
        }
        return null;
    }
    public async Task<IEnumerable<T>> GetAllRecordsAsync1<T>() where T : class
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        return await connection.QueryAsync<T>("SELECT * FROM " + typeof(T).Name);
    }
    public async Task<T> GetRecordByIdAsync<T>(int id) where T : class
    {
        return await Task.Run(() =>
        {
            using var connection = CreateConnection();
            connection.Open();
            return connection.Get<T>(id);
        });
    }

    public async Task InsertRecordAsync<T>(T record) where T : class
    {
        await Task.Run(() =>
        {
            using var connection = CreateConnection();
            connection.Open();
            connection.Insert(record);
        });
    }

    public async Task UpdateRecordAsync<T>(T record) where T : class
    {
        await Task.Run(() =>
        {
            using var connection = CreateConnection();
            connection.Open();
            connection.UpdateAsync(record);
        });
    }

    public async Task UpdateRecordAsync1<T>(T record) where T : class
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        await connection.UpdateAsync(record);
    }
    public async Task DeleteRecordAsync<T>(T record) where T : class
    {
        await Task.Run(() =>
        {
            using var connection = CreateConnection();
            connection.Open();
            connection.Delete(record);
        });
    }

    public bool CheckIfSapCodeExists(string tableName ,double notification)
    {
        using var connection = CreateConnection();
        connection.Open();
        string query = $"SELECT COUNT(*) FROM {tableName} WHERE Notification = @Notification";
        return connection.ExecuteScalar<int>(query ,new { Notification = notification }) > 0;
    }

    public void ClearTable(string tableName)
    {
        using var connection = CreateConnection();
        connection.Open();
        string query = $"DELETE FROM {tableName}";
        connection.Execute(query);
    }

    public async Task<IEnumerable<dynamic>> GetSparePartsWithTechnicianAsync()
    {
        return await Task.Run(() =>
        {
            using var connection = CreateConnection();
            connection.Open();
            string query = @"
                SELECT 
                    sp.PartName, 
                    t.Name, 
                    s.ListName 
                FROM 
                    SparePartsAtTechnicianDatas t
                INNER JOIN 
                    EmployeeDatas sp ON t.PartId = sp.Id
                INNER JOIN 
                    SAPDatas s ON s.Id = t.SapId";
            return connection.Query(query);
        });
    }

    public async Task<List<T>> GetPagedRecordsAsync<T>(string tableName ,int pageNumber ,int pageSize) where T : class
    {
        using var connection = CreateConnection();
        connection.Open();

        int offset = (pageNumber - 1) * pageSize;
        string query = $"SELECT * FROM {tableName} LIMIT @PageSize OFFSET @Offset";

        return (await connection.QueryAsync<T>(query ,new
        {
            PageSize = pageSize ,
            Offset = offset
        })).ToList();
    }

    public async Task<List<T>> GetFilteredRecordsAsync<T>(string tableName ,string filterColumn ,string filterValue) where T : class
    {
        using var connection = CreateConnection();
        connection.Open();

        string query = $"SELECT * FROM {tableName} WHERE {filterColumn} LIKE @FilterValue";
        return (await connection.QueryAsync<T>(query ,new { FilterValue = $"%{filterValue}%" })).ToList();
    }

    public async Task<IEnumerable<dynamic>> GetLinkedRecordsAsync()
    {
        try
        {
            using var connection = CreateConnection();
            connection.Open();

            string query = @"
                        SELECT 
                            v.ID, 
                            v.Notification, 
                            v.VisitDate, 
                            v.Technician,  
                            v.ServiceDetails,  
                            s.ListName 
                        FROM 
                            VisitDatas v
                        INNER JOIN 
                            SAPDatas s ON v.Notification = s.Notification";
            return await connection.QueryAsync(query);
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error DatabaseHelper.GetLinkedRecordsAsync: {ex.Message}");
            return null;
        } 
    }

    public async Task<IEnumerable<dynamic>> GetLinkedRecordsAsync(string filterValue)
    {
        try
        {
            using var connection = CreateConnection();
            connection.Open();

            string query = @"
                        SELECT 
                            v.ID, 
                            v.Notification, 
                            v.VisitDate, 
                            v.Technician,  
                            v.ServiceDetails,  
                            s.ListName 
                        FROM 
                            VisitDatas v
                        INNER JOIN 
                            SAPDatas s ON v.Notification = @FilterValue";

            return (await connection.QueryAsync(query ,new { FilterValue = $"%{filterValue}%" })).ToList();
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error DatabaseHelper.GetLinkedRecordsAsync: {ex.Message}");
            return null;
        } 
    }
}
















//public class DatabaseHelper1

//{
//    private static string _connectionString;
//    private static readonly string ResourcesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
//    private static readonly string ResourcesBackupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
//    private static readonly string dbFilePath = Path.Combine(ResourcesPath ,"AhmedMaintenance.db3");
//    private static readonly string dbbackupPath = Path.Combine(ResourcesBackupPath ,"AhmedMaintenanceBackup.db3");
//    private static readonly string passwordPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
//    private static readonly string passwordFilePath = Path.Combine(passwordPath ,"db_password.txt");

//    public DatabaseHelper1()
//    {
//        // استرجاع كلمة المرور أو إنشاؤها وتشفيرها
//        string dbPassword = GetOrGenerateEncryptedPassword();
//        _connectionString = $"Data Source={dbFilePath};";

//        EnsureDatabaseExists();
//        CreateTables();
//    }

//    private void EnsureDatabaseExists()
//    {
//        try
//        {
//            if(!File.Exists(dbFilePath))
//            {
//                SQLiteConnection.CreateFile(dbFilePath);
//                Debug.WriteLine("تم إنشاء قاعدة البيانات بنجاح.");
//            }
//        }
//        catch(Exception ex)
//        {
//            MessageBox.Show("خطأ أثناء إنشاء قاعدة البيانات: " + ex.Message);
//        }
//    }
//    private string GetOrGenerateEncryptedPassword()
//    {
//        try
//        {
//            return SecureKeyManager.LoadEncryptedKey();
//        }
//        catch(FileNotFoundException)
//        {
//            string newPassword = GenerateRandomPassword();
//            SecureKeyManager.SaveEncryptedKey(newPassword);
//            return newPassword;
//        }
//    }

//    private string GenerateRandomPassword()
//    {
//        return Guid.NewGuid().ToString("N").Substring(0 ,16); // 16 حرفًا عشوائيًا
//    }

//    public static void ChangeDatabasePassword(string newPassword)
//    {
//        try
//        {
//            EncryptionHelper.SaveEncryptedFile(passwordFilePath ,newPassword); // تشفير كلمة المرور الجديدة.
//            using var connection = new SQLiteConnection(_connectionString);
//            connection.Open();

//            using var command = connection.CreateCommand();
//            command.CommandText = "PRAGMA rekey = @newPassword;";
//            command.Parameters.AddWithValue("@newPassword" ,newPassword);
//            command.ExecuteNonQuery();

//            MessageBox.Show("تم تغيير كلمة المرور بنجاح.");
//        }
//        catch(Exception ex)
//        {
//            MessageBox.Show($"حدث خطأ أثناء تغيير كلمة المرور: {ex.Message}");
//        }
//    }

//    private void CreateTables()
//    {
//        using var connection = new SQLiteConnection(_connectionString);
//        connection.Open();
//        using var command = connection.CreateCommand();
//        // بيانات العملاء من الساب
//        string createSAPDatasTable = @"CREATE TABLE IF NOT EXISTS SAPDatas (  Notification DOUBLE PRIMARY KEY, NotificationType TEXT, Region INTEGER, City TEXT, Street TEXT, ListName TEXT, Telephone TEXT, District TEXT, NotifDate TEXT, Description TEXT, Customer INTEGER, MainWorkCtr TEXT, SortField TEXT, BreakdownDuration INTEGER, RequiredEnd TEXT, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER,  ChangeBy INTEGER, Notes TEXT );";
//        // تتميم الفنيين لبلاغات الساب
//        string createVisitDatasTable = @"CREATE TABLE IF NOT EXISTS VisitDatas ( ID INTEGER PRIMARY KEY, Notification DOUBLE,  VisitDate TEXT, TechnicianCode INTEGER, ServiceDetails TEXT, Implemented TEXT,  Paid TEXT, Unpaid TEXT, Completion TEXT,Collector TEXT, Cost REAL, PaymentRefused INTEGER, DistributionStatus TEXT, Notes TEXT, AssistantTechnicianCode INTEGER, DefiningTechnicianCode INTEGER, InstallationDate TEXT, ExecutionDuration INTEGER, WarrantyStatus TEXT, UserStatus TEXT,  DiscountRate REAL,  CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT);";
//        // بيانات قطع الغيار
//        string createSparePartDatasTable = @"CREATE TABLE IF NOT EXISTS SparePartDatas ( SapCode INTEGER PRIMARY KEY AUTOINCREMENT, PartNo TEXT, Group_ TEXT, Model TEXT, DescriptionAR TEXT, DescriptionEN TEXT, C1 TEXT, C2 TEXT, IsDamaged BOOLEAN, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT );";

//        // عهدة الفني
//        string createSparePartsAtTechnicianDatasTable = @"CREATE TABLE IF NOT EXISTS SparePartsAtTechnicianDatas ( ID INTEGER PRIMARY KEY, SapCode INTEGER, TechnicianCode INTEGER, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT );";
//        // قطع الغيار التي تم تركيبها على بلاغ معين
//        string createSparePartsInstalledDatasTable = @"CREATE TABLE IF NOT EXISTS SparePartsInstalledDatas (  ID INTEGER PRIMARY KEY, Notification DOUBLE, SapCode INTEGER, TechnicianCode INTEGER, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT);";
//        // بيانات الموظفين والفنيين
//        string createEmployeeDatasTable = @"CREATE TABLE IF NOT EXISTS EmployeeDatas ( Code INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NULL, Job TEXT NULL,  Branch TEXT NULL, WorkLocation TEXT NULL, Department TEXT NULL, Vendor INTEGER NULL,  CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT );";
//        // جدول ملاحظات للتذكير
//        string createMyNotesDatasTable = @"CREATE TABLE IF NOT EXISTS MyNotesDatas (ID INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT,              Content TEXT, AlertTime DATETIME, Alerted INTEGER, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER,  ChangeBy INTEGER, Notes TEXT );";

//        ExecuteNonQuery(connection ,createSAPDatasTable);
//        ExecuteNonQuery(connection ,createVisitDatasTable);
//        ExecuteNonQuery(connection ,createSparePartsInstalledDatasTable);
//        ExecuteNonQuery(connection ,createEmployeeDatasTable);
//        ExecuteNonQuery(connection ,createSparePartDatasTable);
//        ExecuteNonQuery(connection ,createMyNotesDatasTable);
//        ExecuteNonQuery(connection ,createSparePartsAtTechnicianDatasTable);
//    }

//    private void ExecuteNonQuery(SQLiteConnection connection ,string commandText)
//    {
//        using var command = new SQLiteCommand(commandText ,connection);
//        command.ExecuteNonQuery();
//    }

//    public static IEnumerable<SAPData> GetAllRecords()
//    {
//        var records = new List<SAPData>();
//        using(var connection = new SQLiteConnection(_connectionString))
//        {
//            connection.Open();
//            var command = connection.CreateCommand();
//            command.CommandText = "SELECT * FROM SAPDatas"; // تحقق من صحة الجدول والأعمدة

//            using var reader = command.ExecuteReader();
//            while(reader.Read())
//            {
//                var record = new SAPData
//                {
//                    Notification = reader.IsDBNull(reader.GetOrdinal("Notification"))
//                            ? 0 // قيمة افتراضية في حال كانت القيمة NULL
//                            : reader.GetDouble(reader.GetOrdinal("Notification")) ,

//                    NotificationType = reader.IsDBNull(reader.GetOrdinal("NotificationType"))
//                            ? string.Empty
//                            : reader.GetString(reader.GetOrdinal("NotificationType")) ,

//                    Region = reader.IsDBNull(reader.GetOrdinal("Region"))
//                            ? 0
//                            : reader.GetInt32(reader.GetOrdinal("Region")) ,

//                    City = reader.IsDBNull(reader.GetOrdinal("City"))
//                            ? string.Empty
//                            : reader.GetString(reader.GetOrdinal("City")) ,

//                    Street = reader.IsDBNull(reader.GetOrdinal("Street"))
//                            ? string.Empty
//                            : reader.GetString(reader.GetOrdinal("Street")) ,

//                    ListName = reader.IsDBNull(reader.GetOrdinal("ListName"))
//                            ? string.Empty
//                            : reader.GetString(reader.GetOrdinal("ListName")) ,

//                    Telephone = reader.IsDBNull(reader.GetOrdinal("Telephone"))
//                            ? string.Empty
//                            : reader.GetString(reader.GetOrdinal("Telephone")) ,

//                    District = reader.IsDBNull(reader.GetOrdinal("District"))
//                            ? string.Empty
//                            : reader.GetString(reader.GetOrdinal("District")) ,

//                    NotifDate = reader.IsDBNull(reader.GetOrdinal("NotifDate"))
//                            ? DateTime.MinValue
//                            : ConvertToDateTime(reader.GetValue(reader.GetOrdinal("NotifDate"))) ,

//                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
//                            ? string.Empty
//                            : reader.GetString(reader.GetOrdinal("Description")) ,

//                    Customer = reader.IsDBNull(reader.GetOrdinal("Customer"))
//                            ? 0
//                            : reader.GetInt32(reader.GetOrdinal("Customer")) ,

//                    MainWorkCtr = reader.IsDBNull(reader.GetOrdinal("MainWorkCtr"))
//                            ? string.Empty
//                            : reader.GetString(reader.GetOrdinal("MainWorkCtr")) ,

//                    SortField = reader.IsDBNull(reader.GetOrdinal("SortField"))
//                            ? string.Empty
//                            : reader.GetString(reader.GetOrdinal("SortField")) ,

//                    BreakdownDuration = reader.IsDBNull(reader.GetOrdinal("BreakdownDuration"))
//                            ? 0
//                            : reader.GetInt32(reader.GetOrdinal("BreakdownDuration")) ,

//                    RequiredEnd = reader.IsDBNull(reader.GetOrdinal("RequiredEnd"))
//                            ? DateTime.MinValue
//                            : ConvertToDateTime(reader.GetValue(reader.GetOrdinal("RequiredEnd")))
//                };
//                records.Add(record);
//            }
//        }
//        return records;
//    }

//    private static DateTime ConvertToDateTime(object value)
//    {
//        if(value is DateTime date)
//        {
//            return date; // إذا كانت القيمة هي بالفعل DateTime
//        }
//        else if(value is string dateString)
//        {
//            // محاولة تحويل التاريخ من النص
//            DateTime.TryParse(dateString ,out DateTime parsedDate);
//            return parsedDate; // يمكن أن تكون القيمة غير صالحة فتعود قيمة فارغة
//        }
//        else if(value is long unixTime) // في حال كانت القيمة عدد ثواني (التاريخ كنظام UNIX)
//        {
//            return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
//        }
//        return DateTime.MinValue; // في حال كانت القيمة غير صالحة
//    }

//    // حفظ سجل جديد
//    public static void SaveRecord(VisitData record)
//    {
//        using var connection = new SQLiteConnection(_connectionString);
//        connection.Open();
//        var command = connection.CreateCommand();
//        command.CommandText =
//        @"INSERT INTO VisitRecords (VisitDate, Technician, ServiceDetails, Cost) VALUES (@VisitDate, @Technician, @ServiceDetails, @Cost)";
//        command.Parameters.AddWithValue("@VisitDate" ,record.VisitDate);
//        command.Parameters.AddWithValue("@Technician" ,record.Technician);
//        command.Parameters.AddWithValue("@ServiceDetails" ,record.ServiceDetails);
//        command.Parameters.AddWithValue("@Cost" ,record.Cost);
//        command.ExecuteNonQuery();
//    }

//    public static void ExecuteQuery(string query ,object parameters = null)
//    {
//        using var connection = new SQLiteConnection(_connectionString);
//        connection.Open();
//        connection.Execute(query ,parameters);
//    }

//    public static bool CheckIfSapCodeExists(string tableName ,double notification)
//    {
//        using var connection = new SQLiteConnection(_connectionString);
//        connection.Open();
//        string query = $"SELECT COUNT(*) FROM {tableName} WHERE Notification = @Notification";
//        using var command = new SQLiteCommand(query ,connection);
//        command.Parameters.AddWithValue("@Notification" ,notification);
//        return Convert.ToDouble(command.ExecuteScalar()) > 0;
//    }

//    public void ClearTable(string tableName)
//    {
//        try
//        {
//            using var connection = new SQLiteConnection(_connectionString);
//            connection.Open();

//            string clearQuery = $"DELETE FROM {tableName};";
//            using var command = new SQLiteCommand(clearQuery ,connection);
//            command.ExecuteNonQuery(); MessageBox.Show($"تم تفريغ جدول {tableName} بنجاح.");
//        }
//        catch(Exception ex)
//        {
//            MessageBox.Show($"حدث خطأ أثناء تفريغ جدول {tableName}: " + ex.Message);
//        }
//    }

//    /// <summary>
//    /// Deletes the database file permanently.
//    /// </summary>
//    public static void DeleteDatabase()
//    {
//        try
//        {
//            if(File.Exists(dbFilePath))
//            {
//                File.Delete(dbFilePath);
//                MessageBox.Show("تم حذف قاعدة البيانات بنجاح. " + dbFilePath);
//            }
//            else
//            {
//                MessageBox.Show("لم يتم حذف قاعدة.");
//            }
//        }
//        catch(Exception ex)
//        {
//            Console.WriteLine($"Error deleting database: {ex.Message}");
//        }
//    }

//    public static void InsertIntoTable(string tableName ,Dictionary<string ,object> parameters)
//    {
//        try
//        {
//            using var connection = new SQLiteConnection(_connectionString);
//            connection.Open();

//            var columns = string.Join(", " ,parameters.Keys);
//            var values = string.Join(", " ,parameters.Keys.Select(key => $"@{key}"));
//            var insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

//            using var command = new SQLiteCommand(insertQuery ,connection);
//            foreach(var param in parameters)
//            {
//                command.Parameters.AddWithValue($"@{param.Key}" ,param.Value ?? DBNull.Value);
//            }
//            command.ExecuteNonQuery();
//        }
//        catch(SQLiteException sqlEx)
//        {
//            MessageBox.Show($"خطأ في SQLite: {sqlEx.Message}");
//        }
//        catch(Exception ex)
//        {
//            MessageBox.Show($"خطأ عام: {ex.Message}");
//        }
//    }

//    public void AddRecord(VisitData record)
//    {
//        using var connection = new SQLiteConnection(_connectionString);
//        connection.Execute(@"INSERT INTO VisitDatas ( VisitDate, Technician, ServiceDetails, CompletionDetails, Paid, Cost, PaymentRefused, Notes)VALUES ( @VisitDate, @Technician, @ServiceDetails, @CompletionDetails, @Paid, @Cost, @PaymentRefused, @Notes)" , record);
//    }
//    public static void ImportDatabase()
//    {
//        try
//        {
//            File.Copy(dbbackupPath ,dbFilePath ,overwrite: true);
//            MessageBox.Show("تم استيراد قاعدة البيانات بنجاح من " + dbbackupPath);
//        }
//        catch(Exception ex)
//        {
//            MessageBox.Show("حدث خطأ أثناء استيراد قاعدة البيانات: " + ex.Message);
//        }
//    }

//    public static void ExportDatabase()
//    {
//        try
//        {
//            File.Copy(dbFilePath ,dbbackupPath ,overwrite: true);
//            MessageBox.Show("تم تصدير قاعدة البيانات بنجاح إلى " + dbbackupPath);
//        }
//        catch(Exception ex)
//        {
//            MessageBox.Show("حدث خطأ أثناء تصدير قاعدة البيانات: " + ex.Message);
//        }
//    }
//}
