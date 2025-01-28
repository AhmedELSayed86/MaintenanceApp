using MaintenanceApp.WPF.Models;
using System.Collections.ObjectModel;

namespace MaintenanceApp.WPF.ViewModels;

public class AboutViewModel
{
    public string AppName { get; set; }
    public string AppVersion { get; set; }
    public string AppDescription { get; set; }
    public ObservableCollection<LibraryInfo> Libraries { get; set; }

    public AboutViewModel()
    {
        // تعيين معلومات التطبيق
        AppName = "تطبيق إدارة الصيانة";
        AppVersion = "الإصدار 1.0.0";
        AppDescription = "تطبيق لإدارة أعمال الصيانة وتوزيع المهام على الفنيين.";

        // تعيين معلومات المكتبات المستخدمة
        Libraries =
            [
                new LibraryInfo { Name = "NLog", License = "BSD 3-Clause", Description = "مكتبة لتسجيل الأخطاء." },
                new LibraryInfo { Name = "Newtonsoft.Json", License = "MIT", Description = "مكتبة لمعالجة JSON." },
                new LibraryInfo { Name = "Dapper", License = "Apache 2.0", Description = "مكتبة ORM خفيفة الوزن." }
            ];
    }
}
