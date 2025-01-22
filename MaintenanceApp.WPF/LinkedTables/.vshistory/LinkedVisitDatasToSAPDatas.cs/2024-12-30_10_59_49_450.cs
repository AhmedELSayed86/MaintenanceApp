using Dapper.Contrib.Extensions;
using System;

namespace MaintenanceApp.WPF.LinkedTables;

public class LinkedVisitDatasToSAPDatas
{
    [Key]
    public int ID { get; set; }
    public double Notification { get; set; }
    public DateTime VisitDate { get; set; }
    public string Technician { get; set; }
    public string ServiceDetails { get; set; }
    public string ListName { get; set; }
}
