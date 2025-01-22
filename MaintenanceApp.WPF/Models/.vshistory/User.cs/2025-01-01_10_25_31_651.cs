using Dapper.Contrib.Extensions;
using System;

namespace MaintenanceApp.WPF.Models;

[Table("Users")]
public class User
{    /// <summary>
     /// رقم مسلسل
     /// </summary>
    [Key]
    public string ID { get; set; }
    /// <summary>
     /// اسم المستخدم 
     /// </summary>
    [Key]
    public string UserName { get; set; }
    /// <summary>
    /// الاسم الاول
    /// </summary>
    public string FirstName { get; set; }
    /// <summary>
    /// الاسم الثاني
    /// </summary>
    public string LastName { get; set; }
    /// <summary>
    /// الاسم الثاني
    /// </summary>
    public string PasswordHash { get; set; }
    /// <summary>
    /// الاسم الثاني
    /// </summary>
    public string Role { get; set; }
    /// <summary>
    /// انشئ في  تاريخ
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// تغير في  تاريخ
    /// </summary>
    public DateTime ChangeOn { get; set; }

    /// <summary>
    /// انشئ بواسطة   
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// تغير بواسطة   
    /// </summary>
    public int ChangeBy { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string Notes { get; set; }
}
