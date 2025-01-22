﻿using Dapper;
using Dapper.Contrib.Extensions;
using MaintenanceApp.WPF.Controllers;
using MaintenanceApp.WPF.LinkedTables;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using User = MaintenanceApp.WPF.Models.User;

namespace MaintenanceApp.WPF.Helper;

public class DatabaseHelper : IDatabaseHelper, IDisposable
{
    private readonly SQLiteConnection _connection;
    private readonly string _connectionString;
   
    private static readonly string ResourcesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
    private static readonly string ResourcesPath1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
    private static readonly string dbFilePath = Path.Combine(ResourcesPath ,"MaintenanceApp.db3");
    private static readonly string backupPath = Path.Combine(ResourcesPath1 ,"MaintenanceAppBackup.db3");

    private SQLiteConnection CreateConnection() => new(_connectionString);
    public DatabaseHelper(string password)
    {
        // تشفير قاعدة البيانات باستخدام كلمة المرور
        _connectionString = $"Data Source={dbFilePath};Version=3;Password={password};";
        _connection = new SQLiteConnection(_connectionString);
        InitializeDatabase().Wait(); // انتظر اكتمال التهيئة
    }
     
    public void Dispose()
    {
        _connection?.Dispose();
    }

    private async Task InitializeDatabase()
    {
        if(!File.Exists(dbFilePath))
        {
            SQLiteConnection.CreateFile(dbFilePath); // إنشاء قاعدة بيانات جديدة
        }

        await CreateTablesAsync();
        await SetupIndexesAndConstraintsAsync();
        await ALTERTABLEAsync();
        await AddTestUserAsync();
    }
     
    private async Task CreateTablesAsync()
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
      
        // بيانات المستخدمين
        string createUsersTable = @"CREATE TABLE IF NOT EXISTS Users (
                                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                        UserName NVARCHAR(50) UNIQUE NOT NULL,
                                        FirstName NVARCHAR(50) NOT NULL,
                                        LastName NVARCHAR(50) NOT NULL,
                                        PasswordHash NVARCHAR(256) NOT NULL,
                                        Role NVARCHAR(50) NOT NULL,
                                        IsActive INTEGER,
                                        DeactivatedOn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                        CreatedOn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                        ChangeOn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                        CreatedBy INTEGER,
                                        ChangeBy INTEGER,
                                        Notes BLOB NULL
                                    );";
        // بيانات العملاء من الساب
        string createSAPDatasTable = @"CREATE TABLE IF NOT EXISTS SAPDatas (  Notification DOUBLE PRIMARY KEY, NotificationType TEXT, Region INTEGER, City TEXT, Street TEXT, ListName TEXT, Telephone TEXT, District TEXT, NotifDate TEXT, Description TEXT, Customer INTEGER, MainWorkCtr TEXT, SortField TEXT, BreakdownDuration INTEGER, RequiredEnd TEXT,StatusID INTEGER, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER,  ChangeBy INTEGER, Notes TEXT );";
        // بيانات العملاء من الساب
        string createStatusesTable = @"CREATE TABLE IF NOT EXISTS Statuses (  ID INTEGER PRIMARY KEY, Name TEXT,SAPNO INTEGER,DiscriptionAR TEXT,DiscriptionEN TEXT, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT );";
        // تتميم الفنيين لبلاغات الساب
        string createVisitDatasTable = @"CREATE TABLE IF NOT EXISTS VisitDatas ( ID INTEGER PRIMARY KEY, Notification DOUBLE,  VisitDate TEXT, Technician INTEGER, ServiceDetails TEXT, Implemented TEXT,  Paid TEXT, Unpaid TEXT, Cost REAL, PaymentRefused INTEGER, DistributionStatus TEXT, AssistantTechnician INTEGER, DeterminationTechnician INTEGER, InstallationDate TEXT, ExecutionDuration INTEGER, WarrantyStatus TEXT, UserStatus TEXT,  DiscountPercentage REAL,  CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT);";
        // بيانات قطع الغيار
        string createSparePartDatasTable = @"CREATE TABLE IF NOT EXISTS SparePartDatas ( SapCode INTEGER PRIMARY KEY AUTOINCREMENT, PartNo TEXT, Group_ TEXT, Model TEXT, DescriptionAR TEXT, DescriptionEN TEXT, C1 TEXT, C2 TEXT, IsDamaged BOOLEAN, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT );";
        // عهدة الفني
        string createSparePartsAtTechnicianDatasTable = @"CREATE TABLE IF NOT EXISTS SparePartsAtTechnicianDatas ( ID INTEGER PRIMARY KEY, SapCode INTEGER, Technician INTEGER, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT );";
        // قطع الغيار التي تم تركيبها على بلاغ معين
        string createSparePartsInstalledDatasTable = @"CREATE TABLE IF NOT EXISTS SparePartsInstalledDatas (  ID INTEGER PRIMARY KEY, Notification DOUBLE, SapCode INTEGER, Technician INTEGER, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT);";
        // بيانات الموظفين والفنيين
        string createEmployeeDatasTable = @"CREATE TABLE IF NOT EXISTS EmployeeDatas ( Code INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NULL, Job TEXT NULL,  Branch TEXT NULL, WorkLocation TEXT NULL, Department TEXT NULL, Vendor INTEGER NULL,  CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER, ChangeBy INTEGER, Notes TEXT );";
        // جدول ملاحظات للتذكير
        string createMyNotesDatasTable = @"CREATE TABLE IF NOT EXISTS MyNotesDatas (ID INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT,              Content TEXT, AlertTime DATETIME, Alerted INTEGER, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER,  ChangeBy INTEGER, Notes TEXT );";

        ExecuteNonQuery(connection ,createUsersTable);
        ExecuteNonQuery(connection ,createSAPDatasTable);
        ExecuteNonQuery(connection ,createStatusesTable);
        ExecuteNonQuery(connection ,createVisitDatasTable);
        ExecuteNonQuery(connection ,createSparePartsInstalledDatasTable);
        ExecuteNonQuery(connection ,createEmployeeDatasTable);
        ExecuteNonQuery(connection ,createSparePartDatasTable);
        ExecuteNonQuery(connection ,createMyNotesDatasTable);
        ExecuteNonQuery(connection ,createSparePartsAtTechnicianDatasTable);
    }

    private async Task ExecuteNonQuery(SQLiteConnection connection ,string commandText)
    {
        using var command = new SQLiteCommand(commandText ,connection);
        await command.ExecuteNonQueryAsync();
    }

    private async Task ALTERTABLE()
    {
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            var constraintQueries = new List<string>
            {
                // إضافة اعمدة جديدة الى الجدول مثل CHECK
                "ALTER TABLE IF NOT EXISTS Users ADD COLUMN IsActive INTEGER;",
                "ALTER TABLE IF NOT EXISTS Users ADD COLUMN DeactivatedOn DATETIME;",
                "ALTER TABLE IF NOT EXISTS Users ADD COLUMN StatusID INTEGER;"
                // أضف المزيد حسب الحاجة
            };

            foreach(var query in constraintQueries)
            {
                await ExecuteNonQueryAsync(connection ,query);
            }
        }
        catch(SQLiteException ex)
        {
            // قد تحدث الأخطاء إذا كانت القيود موجودة بالفعل
       await   MessageController.SummaryAsync($"Constraint Error: {ex.Message}",Brushes.IndianRed);
        }
        catch(Exception ex)
        {
            // قد تحدث الأخطاء إذا كانت القيود موجودة بالفعل
            await MessageController.SummaryAsync($"Constraint Error: {ex.Message}" ,Brushes.IndianRed);
        }
    }

    private void SetupIndexesAndConstraints()
    {
        using var connection = CreateConnection();
        connection.Open();

        var indexQueries = new List<string>
            {
                // إضافة الفهرسة لجداول مختلفة
                "CREATE INDEX IF NOT EXISTS IX_Users_Role ON Users(Role);",
                "CREATE INDEX IF NOT EXISTS IX_SAPDatas_Customer ON SAPDatas(Customer);"
                // أضف المزيد حسب الحاجة
            };

        var constraintQueries = new List<string>
            {
                // إضافة قيود مثل CHECK
                "ALTER TABLE Users ADD CONSTRAINT IF NOT EXISTS CK_Users_Role CHECK (Role IN ('Admin', 'Technician', 'Viewer'));"
                // أضف المزيد حسب الحاجة
            };

        foreach(var query in indexQueries)
        {
            ExecuteNonQuery(connection ,query);
        }

        foreach(var query in constraintQueries)
        {
            try
            {
                ExecuteNonQuery(connection ,query);
            }
            catch(SQLiteException ex)
            {
                // قد تحدث الأخطاء إذا كانت القيود موجودة بالفعل
                Console.WriteLine($"Constraint Error: {ex.Message}");
            }
        }
    }

    public async Task ExecuteInTransactionAsync(Func<IDbConnection ,IDbTransaction ,Task> action)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();
        try
        {
            await action(connection ,transaction);
            transaction.Commit(); // تأكيد المعاملة إذا نجحت جميع العمليات
        }
        catch
        {
            transaction.Rollback(); // التراجع عن المعاملة في حالة حدوث خطأ
            throw; // إعادة رمي الاستثناء للتعامل معه في الكود الذي يستدعي الدالة
        }
    }

    public async Task<bool> CheckIfSapCodeExistsAsync(string tableName ,string filterColumn ,string sapCode)
    {
        using var connection = CreateConnection();
        connection.Open();

        // بناء الاستعلام بشكل صحيح باستخدام اسم العمود كجزء من النص الثابت
        var query = $"SELECT COUNT(*) FROM {tableName} WHERE {filterColumn} = @sapCode";

        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@sapCode" ,sapCode);

        // تنفيذ الاستعلام والحصول على النتيجة
        var result = await Task.Run(() => command.ExecuteScalar());

        // التحقق من النتيجة
        return result != null && Convert.ToInt32(result) > 0;
    }

    /// <summary>
    /// Excel اضافة
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public async Task ExcelInsertIntoTableAsync(string tableName ,Dictionary<string ,object> parameters)
    {
        try
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
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message ,"Error");
        }
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
    /// <summary>
    /// جلب اسم الحالة FIVI
    /// </summary>
    /// <returns></returns>
    public async Task<List<SAPDataWithStatus>> GetSAPDataWithStatusAsync()
    {
        try
        {
            return await Task.Run(() =>
            {
                using var connection = CreateConnection();
                connection.Open();

                string query = @"
                SELECT 
                    s.*, 
                    st.Name AS StatusName 
                FROM 
                    SAPDatas s 
                INNER JOIN 
                    Statuses st ON s.StatusID = st.ID";

                return connection.Query<SAPDataWithStatus>(query).AsList();
            });
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error GetSAPDataWithStatusAsync: {ex.Message}");
            return null;
        }
    }
    /// <summary>
    ///  // بناء الاستعلام مع استخدام NOT IN لاستبعاد القيم المحددة
    /// </summary>
    /// <returns></returns>
    public async Task<List<SAPDataWithStatus>> GetFilteredSAPDataAsync()
    {
        try
        {
            return await Task.Run(() =>
            {
                using var connection = CreateConnection();
                connection.Open();

                // قائمة القيم التي تريد استبعادها من عمود StatusID
                var excludedStatusIDs = new List<int> { 2 ,3 ,4 ,5 ,6 ,7 ,8 ,9 ,10 ,15 ,18 };

                // بناء الاستعلام مع استخدام NOT IN لاستبعاد القيم المحددة
                string query = @"
            SELECT 
                s.*, 
                st.Name AS StatusName 
            FROM 
                SAPDatas s 
            INNER JOIN 
                Statuses st ON s.StatusID = st.ID
            WHERE 
                s.StatusID NOT IN @ExcludedStatusIDs";

                // تنفيذ الاستعلام مع تمرير القيم المستبعدة كمعلمة
                return connection.Query<SAPDataWithStatus>(query ,new { ExcludedStatusIDs = excludedStatusIDs }).AsList();
            });
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error in GetFilteredSAPDataAsync: {ex.Message}");
            return null;
        }
    }
    /// <summary>
    ///   بناء الاستعلام مع استخدام 0
    /// </summary>
    /// <returns></returns>
    public async Task<List<SAPDataWithStatus>> GetFilteredSAPData0Async()
    {
        try
        {
            return await Task.Run(() =>
            {
                using var connection = CreateConnection();
                connection.Open();

                // قائمة القيم التي تريد استبعادها من عمود StatusID
                var excludedStatusIDs = new List<int> { 0 };

                // بناء الاستعلام مع استخدام NOT IN لاستبعاد القيم المحددة
                string query = $"SELECT * FROM SAPDatas WHERE StatusID = 0";

                //string query = @"
                //SELECT 
                //    s.*
                //FROM 
                //    SAPDatas s            
                //WHERE 
                //    s.StatusID NOT IN @ExcludedStatusIDs";

                // تنفيذ الاستعلام مع تمرير القيم المستبعدة كمعلمة
                return connection.Query<SAPDataWithStatus>(query ,new { ExcludedStatusIDs = excludedStatusIDs }).AsList();
            });
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error in GetFilteredSAPDataAsync: {ex.Message}");
            return null;
        }
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

    /// <summary>
    /// اضافة 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="record"></param>
    /// <returns></returns>
    public async Task InsertRecordAsync<T>(T record) where T : class
    {
        try
        {
            await Task.Run(() =>
               {
                   using var connection = CreateConnection();
                   connection.Open();
                   connection.Insert(record);
               });
        }
        catch(Exception ex)
        {
            MessageBox.Show($"ERROR InsertRecordAsync:{record}. \n{ex.Message}");
        }
    }

    public async Task UpdateRecordAsync<T>(T record) where T : class
    {
        await Task.Run(() =>
        {
            using var connection = CreateConnection();
            connection.Open();
            connection.Update<T>(record); // تأكد من استخدام Update بدلاً من UpdateAsync إذا كنت تستخدم Dapper
        });
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
                    EmployeeDatas sp ON t.PartId = sp.Code
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

    public async Task<IEnumerable<LinkedVisitDatasToSAPDatas>> GetFilteredLinkedRecordsAsync(
        string table1 ,string table2 ,string table3 ,string joinColumn ,string filterValue)
    {
        try
        {
            using var connection = CreateConnection();
            connection.Open();

            string query = $@"
            SELECT 
                v.ID, 
                v.Notification, 
                v.VisitDate, 
                v.Technician, 
                v.ServiceDetails, 
                s.ListName, 
                e.Name AS TechnicianName
            FROM 
                {table1} v
            INNER JOIN 
                {table2} s ON v.{joinColumn} = s.{joinColumn}
            INNER JOIN 
                {table3} e ON v.Technician = e.Code
            WHERE 
                v.{joinColumn} = @FilterValue";

            return await connection.QueryAsync<LinkedVisitDatasToSAPDatas>(query ,new { FilterValue = filterValue });
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error in GetFilteredLinkedRecordsAsync: {ex.Message}");
            return null;
        }
    }
    /// <summary>
    /// اضافة مستخدم للاختبار
    /// </summary>
    /// <returns></returns>
    public async Task<bool> AddTestUserAsync()
    {
        try
        {
            // حفظ كلمة المرور المشفرة في ملف
            string password = "MySecurePassword123";
            PasswordManager.SaveEncryptedPassword(password);

            // تحميل كلمة المرور المشفرة من الملف
            string decryptedPassword = PasswordManager.LoadEncryptedPassword();

            // استخدام كلمة المرور لتشفير قاعدة البيانات
            using var dbHelper = new DatabaseHelper(decryptedPassword);

            // التحقق من وجود المستخدم
            if(await UserExistsAsync("AHMED"))
            {
                await MessageController.SummaryAsync("المستخدم موجود بالفعل.");
                return false;
            }

            // إضافة المستخدم الجديد
            var newUser = CreateTestUser();
            await InsertUserAsync(newUser);

            await MessageController.SummaryAsync("تمت إضافة المستخدم التجريبي بنجاح.");
            return true;
        }
        catch(Exception ex)
        {
            await MessageController.SummaryAsync($"خطأ أثناء إضافة المستخدم التجريبي: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> UserExistsAsync(string username)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        var userExists = await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE UserName = @Username" ,
            new { Username = username });

        return userExists != null;
    }

    private User CreateTestUser()
    {
        return new User
        {
            UserName = "AHMED" ,
            PasswordHash = HashHelper.HashPassword("123456") ,
            FirstName = "Ahmed" ,
            LastName = "EL-Sayed" ,
            Role = "Admin" ,
            IsActive = true ,
            DeactivatedOn = DateTime.UtcNow ,
            CreatedOn = DateTime.UtcNow ,
            ChangeOn = DateTime.UtcNow ,
            CreatedBy = 1 ,
            Notes = "تم الإنشاء بواسطة النظام"
        };
    }

    private async Task InsertUserAsync(User user)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        await connection.InsertAsync(user);
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        using var connection = new SQLiteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE UserName = @Username" ,
            new { Username = username });
    }
}
