using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceApp.WPF.Models
{
    public class Status
    {using Dapper.Contrib.Extensions;
using System;
using System.Collections.ObjectModel;

namespace MaintenanceApp.WPF.Models;

    [Table("SAPDatas")]
    public class SAPData
    {
        /// <summary>
        /// رقم البلاغ
        /// </summary>
        [Dapper.Contrib.Extensions.Key]
        public double Notification { get; set; }

        /// <summary>
        /// نوع البلاغ
        /// </summary>
        public string NotificationType { get; set; }

        /// <summary>
        /// المحافظة
        /// </summary>
        public int? Region { get; set; }

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
        public int? Customer { get; set; }

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
        public int? BreakdownDuration { get; set; }

        /// <summary>
        /// النهاية المطلوبة
        /// </summary>
        public DateTime RequiredEnd { get; set; }

        /// <summary>
        /// حالة الاوردر
        /// </summary>
        public int Status { get; set; }

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
        public ObservableCollection<SAPData> Visits { get; set; } = [];
    }
    /*
    Notification NotificationType Region City Street ListName Telephone District NotifDate Description Customer MainWorkCtr SortField BreakdownDuration RequiredEnd VisitDate Technician ServiceDetails Implemented Paid Unpaid Cost PaymentRefused DistributionStatus Notes AssistantTechnician DeterminationTechnician InstallationDate ExecutionDuration WarrantyStatus UserStatus DiscountPercentage 
        */
   
}
