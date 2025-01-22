using MaintenanceApp.WPF.LinkedTables;
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
    void ClearTable(string tableName);  
    Task<IEnumerable<dynamic>> GetSparePartsWithTechnicianAsync();
    Task<bool> CheckIfSapCodeExistsAsync(string tableName ,string filterColumn ,double sapCode);
    Task InsertIntoTableAsync(string tableName ,Dictionary<string ,object> parameters);
    Task<List<T>> GetPagedRecordsAsync<T>(string tableName ,int pageNumber ,int pageSize) where T : class;
    Task<List<T>> GetFilteredRecordsAsync<T>(string tableName ,string filterColumn ,string filterValue) where T : class;
    Task<IEnumerable<dynamic>> GetLinkedRecordsAsync();
    Task<IEnumerable<LinkedVisitDatasToSAPDatas>> GetFilteredLinkedRecordsAsync(string table1 ,string table2,string table3 ,string joinColumn1,string filterValue);
}