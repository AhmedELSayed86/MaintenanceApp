using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MaintenanceApp.WPF.Converters;

public class DateConverter : IValueConverter
{
    public object Convert(object value ,Type targetType ,object parameter ,CultureInfo culture)
    {
        if(value is DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        return string.Empty;
    }

    public object ConvertBack(object value ,Type targetType ,object parameter ,CultureInfo culture)
    {
        if(DateTime.TryParse(value as string ,out DateTime result))
        {
            return result;
        }
        return DateTime.MinValue;
    }
}
