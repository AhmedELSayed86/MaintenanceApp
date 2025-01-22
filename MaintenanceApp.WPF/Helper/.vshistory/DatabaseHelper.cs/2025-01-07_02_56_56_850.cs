using Dapper;
using Dapper.Contrib.Extensions;
using MaintenanceApp.WPF.LinkedTables;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using User = MaintenanceApp.WPF.Models.User;

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
        SetupIndexesAndConstraints();
        _ = AddUserAsync();
    }

    private SQLiteConnection CreateConnection() => new SQLiteConnection(_connectionString);

    private void CreateTables()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
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
                                        Notes TEXT
                                    );";
        // بيانات العملاء من الساب
        string createSAPDatasTable = @"CREATE TABLE IF NOT EXISTS SAPDatas (  Notification DOUBLE PRIMARY KEY, NotificationType TEXT, Region INTEGER, City TEXT, Street TEXT, ListName TEXT, Telephone TEXT, District TEXT, NotifDate TEXT, Description TEXT, Customer INTEGER, MainWorkCtr TEXT, SortField TEXT, BreakdownDuration INTEGER, RequiredEnd TEXT, CreatedOn TEXT, ChangeOn TEXT, CreatedBy INTEGER,  ChangeBy INTEGER, Notes TEXT );";
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

    public async Task<bool> CheckIfSapCodeExistsAsync(string tableName ,string filterColumn ,double sapCode)
    {
        using var connection = CreateConnection();
        connection.Open();

        var query = $"SELECT COUNT(1) FROM {tableName} WHERE @filterColumn = @sapCode";
        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@filterColumn" ,filterColumn);
        command.Parameters.AddWithValue("@sapCode" ,sapCode);

        var result = await Task.Run(() => Convert.ToInt32(command.ExecuteScalar()));
        return result > 0;
    }
    /// <summary>
    /// Excel اضافة
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
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
    /// <exception cref="Exception"></exception>
    public async Task<bool> AddTestUserAsync()
    {
        try
        {
            await Task.Run(() =>
          {
              using var connection = CreateConnection();
              connection.Open();
              var isuser = connection.GetAll<User>().Where(x => x.UserName == "AHMED");
              if(isuser.Any())
              {
                  MessageBox.Show(isuser.FirstOrDefault().UserName);
                  return;
              }

              var hashedPassword = HashHelper.HashPassword("123456");
              var newUser = new User
              {
                  UserName = "Ahmed" ,
                  PasswordHash = hashedPassword
              };

              _ = InsertRecordAsync(newUser);
          });
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error AddTestUserAsync: {ex.Message}");
        }
        return true;
    }

    public async Task<User> GetUserAsync(string username ,string passwordHash)
    {
        using(var connection = new SQLiteConnection(_connectionString))
        {
            return await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE UserName = @Username AND PasswordHash = @PasswordHash" ,
                new { Username = username ,PasswordHash = passwordHash });
        }
    }
}