﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MaintenanceApp.WPF.Converters;

public class TextToVisibilityConverter : IValueConverter
{
    public object Convert(object value ,Type targetType ,object parameter ,CultureInfo culture)
    {
        return string.IsNullOrWhiteSpace(value?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value ,Type targetType ,object parameter ,CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    //public object Convert(object value ,Type targetType ,object parameter ,CultureInfo culture)
    //{
    //    return string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;
    //}

    //public object ConvertBack(object value ,Type targetType ,object parameter ,CultureInfo culture)
    //{
    //    throw new NotImplementedException();
    //}
}