﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MaintenanceApp.WPF.Converters;
            
public class BooleanToVisibilityConverter : IValueConverter  
{
    public object Convert(object value ,Type targetType ,object parameter ,CultureInfo culture)
    {
        return (value is bool boolValue && boolValue) ? Visibility.Collapsed: Visibility.Visible ;
    }

    public object ConvertBack(object value ,Type targetType ,object parameter ,CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
