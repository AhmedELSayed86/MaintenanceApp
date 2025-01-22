using Dapper.Contrib.Extensions;
using System;

namespace MaintenanceApp.WPF.Models;

[Table("MyNotesDatas")]
public class MyNotesData
{
    /// <summary>
    /// الرقم المسلسل
    /// </summary>
    [Key]
    public int ID { get; set; }
    /// <summary>
    /// العنوان
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// المحتوى
    /// </summary>
    public string Content { get; set; }
    /// <summary>
    /// وقت التنبيه
    /// </summary>
    public DateTime AlertTime { get; set; }
    /// <summary>
    /// تم التنبيه
    /// </summary>
    public int Alerted { get; set; }

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