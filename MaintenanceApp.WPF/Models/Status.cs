using Dapper.Contrib.Extensions;
using System;
using System.Collections.ObjectModel;

namespace MaintenanceApp.WPF.Models;

[Table("Statuses")]
public class Status
{
    /// <summary>
    /// رقم البلاغ
    /// </summary>
    [Dapper.Contrib.Extensions.Key]
    public int ID { get; set; }

    /// <summary>
    /// حالة الاوردر
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// رقمه بالساب 
    /// </summary>
    public int SAPNO { get; set; }
    /// <summary>
    /// الوصف العربي
    /// </summary>
    public string DiscriptionAR { get; set; }
    /// <summary>
    /// الوصف الانجليزي 
    /// </summary>
    public string DiscriptionEN { get; set; }

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

    /// <summary>
    /// الربط بين جدول SAPData
    /// </summary>
    public ObservableCollection<SAPData> SAPDetails { get; set; } = [];
}