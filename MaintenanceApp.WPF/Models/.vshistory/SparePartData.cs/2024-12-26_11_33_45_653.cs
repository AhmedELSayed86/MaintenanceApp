using Dapper.Contrib.Extensions;
using System;

namespace MaintenanceApp.WPF.Models;

[Table(" ")]
public class SparePartData
{
    /// <summary>
    /// الرقم المسلسل
    /// </summary>
    [Key]
    public int SapCode { get; set; }

    /// <summary>
    /// رقم القطعة الاصلي  
    /// </summary>
    public string PartNo { get; set; }

    /// <summary>
    /// المجموعة التابع لها
    /// </summary>
    public string MatrialGroup { get; set; }

    /// <summary>
    /// موديل الجهاز
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// وصف عربي
    /// </summary>
    public string DescriptionAR { get; set; }

    /// <summary>
    /// وصف انجليزي
    /// </summary>
    public string DescriptionEN { get; set; }

    /// <summary>
    /// سعر C1
    /// </summary>
    public string C1 { get; set; }

    /// <summary>
    /// سعر C2
    /// </summary>
    public string C2 { get; set; }

    /// <summary>
    /// له تالف
    /// </summary>
    public bool IsDamaged { get; set; }

    /// <summary>
    /// مشروع توحيد الاسماء مثل (كويل - مبخر)
    /// </summary>
    public string ANots { get; set; }
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