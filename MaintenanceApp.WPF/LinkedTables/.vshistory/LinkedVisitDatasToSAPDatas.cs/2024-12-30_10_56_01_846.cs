using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceApp.WPF.LinkedTables;

public class LinkedVisitDatasToSAPDatas
{
    [Key]
    public int ID { get; set; }
    public string Notification { get; set; }
    public DateTime VisitDate { get; set; }
    public string Technician { get; set; }
    public string ServiceDetails { get; set; }
    public string ListName { get; set; }
}
