using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaintenanceApp.WPF.Helper;

public interface IDatabaseHelper
{
    Task<List<T>> GetAllRecordsAsync<T>() where T : class;
    Task<T> GetRecordByIdAsync<T>(int id) where T : class;
    Task InsertRecordAsync<T>(T record) where T : class;
    Task UpdateRecordAsync<T>(T record) where T : class;
    Task DeleteRecordAsync<T>(T record) where T : class;
    bool CheckIfSapCodeExists(string tableName ,double notification);
    void ClearTable(string tableName);
    Task<IEnumerable<dynamic>> GetSparePartsWithTechnicianAsync();
    Task<bool> CheckIfSapCodeExistsAsync(string tableName ,double sapCode);
    Task InsertIntoTableAsync(string tableName ,Dictionary<string ,object> parameters);
}