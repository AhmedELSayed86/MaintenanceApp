using MaintenanceApp.WPF.LinkedTables;
using MaintenanceApp.WPF.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MaintenanceApp.WPF.Helper;

public interface IDatabaseHelper : IDisposable
{
    /// <summary>
    /// تنفيذ عملية في معاملة (transaction).
    /// </summary>
    Task ExecuteInTransactionAsync(Func<IDbConnection ,IDbTransaction ,Task> action);

    /// <summary>
    /// جلب جميع السجلات من الجدول.
    /// </summary>
    Task<List<T>> GetAllRecordsAsync<T>() where T : class;

    /// <summary>
    /// جلب بيانات SAP مع الحالة المرتبطة بها.
    /// </summary>
    Task<List<SAPDataWithStatus>> GetSAPDataWithStatusAsync();

    /// <summary>
    /// جلب سجل بواسطة المعرف.
    /// </summary>
    Task<T> GetRecordByIdAsync<T>(int id) where T : class;

    /// <summary>
    /// جلب مستخدم بواسطة اسم المستخدم.
    /// </summary>
    Task<User> GetUserByUsernameAsync(string username);

    /// <summary>
    /// إدراج سجل جديد في الجدول.
    /// </summary>
    Task InsertRecordAsync<T>(T record) where T : class;

    /// <summary>
    /// تحديث سجل موجود في الجدول.
    /// </summary>
    Task UpdateRecordAsync<T>(T record) where T : class;

    /// <summary>
    /// حذف سجل من الجدول.
    /// </summary>
    Task DeleteRecordAsync<T>(T record) where T : class;

    /// <summary>
    /// مسح جميع السجلات من الجدول.
    /// </summary>
    void ClearTable(string tableName);

    /// <summary>
    /// جلب قطع الغيار مع الفنيين المرتبطين بها.
    /// </summary>
    Task<IEnumerable<dynamic>> GetSparePartsWithTechnicianAsync();

    /// <summary>
    /// التحقق من وجود كود SAP في الجدول.
    /// </summary>
    Task<bool> CheckIfSapCodeExistsAsync(string tableName ,string filterColumn ,string sapCode);

    /// <summary>
    /// إدراج بيانات من Excel في الجدول.
    /// </summary>
    Task ExcelInsertIntoTableAsync(string tableName ,Dictionary<string ,object> parameters);

    /// <summary>
    /// جلب سجلات مقطوعة (paged) من الجدول.
    /// </summary>
    Task<List<T>> GetPagedRecordsAsync<T>(string tableName ,int pageNumber ,int pageSize) where T : class;

    /// <summary>
    /// جلب سجلات مُصفاة من الجدول.
    /// </summary>
    Task<List<T>> GetFilteredRecordsAsync<T>(string tableName ,string filterColumn ,string filterValue) where T : class;

    /// <summary>
    /// جلب سجلات مُصفاة من الجدول وتاريخ اليوم
    /// </summary>
    Task<IEnumerable<LinkedVisitDatasToSAPDatas>> GetFilteredLinkedDateRecordsAsync(string table1 ,string table2 ,string table3 ,string joinColumn ,string filterValue ,DateTime? createdOnDate = null);

    /// <summary>
    /// جلب بيانات SAP مُصفاة.
    /// </summary>
    Task<List<SAPDataWithStatus>> GetFilteredSAPDataAsync();

    /// <summary>
    /// جلب بيانات SAP بحالة محددة.
    /// </summary>
    Task<List<SAPDataWithStatus>> GetFilteredSAPData0Async();

    /// <summary>
    /// جلب سجلات مرتبطة من جداول متعددة.
    /// </summary>
    Task<IEnumerable<dynamic>> GetLinkedRecordsAsync();

    /// <summary>
    /// جلب سجلات مرتبطة مع تصفية.
    /// </summary>
    Task<IEnumerable<LinkedVisitDatasToSAPDatas>> GetFilteredLinkedRecordsAsync(string table1 ,string table2 ,string table3 ,string joinColumn1 ,string filterValue); 
}