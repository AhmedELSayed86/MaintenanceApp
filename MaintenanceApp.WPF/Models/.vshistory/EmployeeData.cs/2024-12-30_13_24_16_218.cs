using Dapper.Contrib.Extensions;
using System;

namespace MaintenanceApp.WPF.Models;

[Table("EmployeeDatas")]
public class EmployeeData
{
    /// <summary>
    /// الرقم المسلسل
    /// </summary>
    [Key]
    public int ID { get; set; } 
    /// الاسم
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// الوظيفة
    /// </summary>
    public string Job { get; set; }
    /// <summary>
    /// الفرع
    /// </summary>
    public string Branch { get; set; }
    /// <summary>
    /// موقع العمل
    /// </summary>
    public string WorkLocation { get; set; }
    /// <summary>
    /// القسم
    /// </summary>
    public string Department { get; set; }
    /// <summary>
    /// كود مالي للمركز
    /// </summary>
    public int Vendor { get; set; }

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