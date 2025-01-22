namespace MaintenanceApp.WPF.Models;

public class SAPDataWithStatus
{
    public double Notification { get; set; }
    public string NotificationType { get; set; }
    public int Region { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string ListName { get; set; }
    public string Telephone { get; set; }
    public string District { get; set; }
    public string NotifDate { get; set; }
    public string Description { get; set; }
    public int Customer { get; set; }
    public string MainWorkCtr { get; set; }
    public string SortField { get; set; }
    public int BreakdownDuration { get; set; }
    public string RequiredEnd { get; set; }
    public int StatusID { get; set; }
    public string CreatedOn { get; set; }
    public string ChangeOn { get; set; }
    public int CreatedBy { get; set; }
    public int ChangeBy { get; set; }
    public string Notes { get; set; }
    public string StatusName { get; set; } // اسم الحالة من جدول Statuses
}
