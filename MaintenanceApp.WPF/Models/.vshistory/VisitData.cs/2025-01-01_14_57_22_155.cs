using Dapper.Contrib.Extensions;
using System;
using System.Collections.ObjectModel;

namespace MaintenanceApp.WPF.Models;

[Table("VisitDatas")]
public class VisitData
{
    /// <summary>
    /// رقم مسلسل
    /// </summary>
    [Dapper.Contrib.Extensions.Key]
    public int ID { get; set; }

    /// <summary>
    /// رقم البلاغ
    /// </summary> 
    public double Notification { get; set; }

    /// <summary>
    /// تاريخ الزيارة
    /// </summary>
    public DateTime VisitDate { get; set; }

    /// <summary>
    /// الفني
    /// </summary>
    public int Technician { get; set; }

    /// <summary>
    /// تفاصيل الخدمة
    /// </summary>
    public string ServiceDetails { get; set; }

    /// <summary>
    /// التتميم/ تم تفيذ
    /// </summary>
    public string Implemented { get; set; }

    /// <summary>
    /// مدفوع
    /// </summary>
    public decimal Paid { get; set; }

    /// <summary>
    /// غير مدفوع
    /// </summary>
    public decimal Unpaid { get; set; }

    /// <summary>
    /// التكلفة
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// رفض دفع
    /// </summary>
    public bool PaymentRefused { get; set; }

    /// <summary>
    /// حالة التوزيع
    /// </summary>
    public string DistributionStatus { get; set; }

    /// <summary>
    /// فني مساعد
    /// </summary>
    public string AssistantTechnician { get; set; }

    /// <summary>
    /// فني التحديد
    /// </summary>
    public string DeterminationTechnician { get; set; }

    /// <summary>
    /// تاريخ التركيب
    /// </summary>
    public DateTime InstallationDate { get; set; }

    /// <summary>
    /// مدة التنفيذ
    /// </summary>
    public string ExecutionDuration { get; set; }

    /// <summary>
    /// حالة الضمان
    /// </summary>
    public bool WarrantyStatus { get; set; }

    /// <summary>
    /// حالة البلاغ
    /// </summary>
    public string UserStatus { get; set; }

    /// <summary>
    /// نسبة الخصم
    /// </summary>
    public decimal DiscountPercentage { get; set; }

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
    public ObservableCollection<EmployeeData> TechnicianName { get; set; } = [];
}
/*
Notification NotificationType Region City Street ListName Telephone District NotifDate Description Customer MainWorkCtr SortField BreakdownDuration RequiredEnd VisitDate Technician ServiceDetails Implemented Paid Unpaid Cost PaymentRefused DistributionStatus Notes AssistantTechnician DeterminationTechnician InstallationDate ExecutionDuration WarrantyStatus UserStatus DiscountPercentage 
    */
/*
/// <summary>
/// رقم البلاغ
/// </summary>
[Key]
public int Notification { get; set; }
/// <summary>
/// نوع البلاغ
/// </summary>
public string NotificationType { get; set; }
/// <summary>
/// المحافظة
/// </summary>
public int Region { get; set; }
/// <summary>
/// المدينة
/// </summary>
public string City { get; set; }
/// <summary>
/// العنوان/شارع
/// </summary>
public string Street { get; set; }
/// <summary>
/// الإسم
/// </summary>
public string ListName { get; set; }
/// <summary>
/// تليفون1
/// </summary>
public string Telephone { get; set; }

/// <summary>
/// تليفون2
/// </summary>
public string District { get; set; }

/// <summary>
/// تاريخ البلاغ
/// </summary>
public DateTime NotifDate { get; set; }

/// <summary>
/// الوصف
/// </summary>
public string Description { get; set; }

/// <summary>
/// العميل
/// </summary>
public int Customer { get; set; }

/// <summary>
/// الورشة
/// </summary>
public string MainWorkCtr { get; set; }

/// <summary>
/// مسلسل رقم/ السيريل
/// </summary>
public string SortField { get; set; }

/// <summary>
/// مدة التنفيذ
/// </summary>
public int BreakdownDuration { get; set; }

/// <summary>
/// النهاية المطلوبة
/// </summary>
public DateTime RequiredEnd { get; set; }

/// <summary>
/// تاريخ الزيارة
/// </summary>
public DateTime VisitDate { get; set; }

/// <summary>
/// الفني
/// </summary>
public string Technician { get; set; }

/// <summary>
/// تفاصيل الخدمة
/// </summary>
public string ServiceDetails { get; set; }

/// <summary>
/// التتميم/ تم تفيذ
/// </summary>
public string Implemented { get; set; }

/// <summary>
/// مدفوع
/// </summary>
public decimal Paid { get; set; }

/// <summary>
/// غير مدفوع
/// </summary>
public decimal Unpaid { get; set; }

/// <summary>
/// التكلفة
/// </summary>
public decimal Cost { get; set; }

/// <summary>
/// رفض دفع
/// </summary>
public bool PaymentRefused { get; set; }

/// <summary>
/// حالة التوزيع
/// </summary>
public string DistributionStatus { get; set; }

/// <summary>
/// ملاحظات
/// </summary>
public string Notes { get; set; }

/// <summary>
/// فني مساعد
/// </summary>
public string AssistantTechnician { get; set; }

/// <summary>
/// فني التحديد
/// </summary>
public string DeterminationTechnician { get; set; }

/// <summary>
/// تاريخ التركيب
/// </summary>
public DateTime InstallationDate { get; set; }

/// <summary>
/// مدة التنفيذ
/// </summary>
public string ExecutionDuration { get; set; }

/// <summary>
/// حالة الضمان
/// </summary>
public bool WarrantyStatus { get; set; }

/// <summary>
/// SAP
/// </summary>
public string SAP { get; set; }
/// <summary>
/// نسبة الخصم
/// </summary>
public decimal DiscountPercentage { get; set; }
*/