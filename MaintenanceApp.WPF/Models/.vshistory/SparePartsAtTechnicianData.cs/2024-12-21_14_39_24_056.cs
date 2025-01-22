using System;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceApp.WPF.Models;

public class SparePartsAtTechnicianData
{
    /// <summary>
    /// الرقم المسلسل
    /// </summary>
    [Key]
    public int ID { get; set; }

    /// <summary>
    /// كود قطعة الغيار
    /// </summary>
    public int SapCode { get; set; }

    /// <summary>
    /// كود الفني
    /// </summary>
    public int TechnicianCode { get; set; }

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