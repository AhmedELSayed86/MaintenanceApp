using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceApp.WPF.Models
{
    [Table("Users")]
    public class User
    {    /// <summary>
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
}
