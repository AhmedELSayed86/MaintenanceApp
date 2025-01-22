using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using System.Windows;
    using System.Windows.Controls;

namespace MaintenanceApp.WPF.Helper;

public static class PasswordBoxHelper
{
    public static readonly DependencyProperty BoundPasswordProperty =
        DependencyProperty.RegisterAttached(
            "BoundPassword" ,
            typeof(string) ,
            typeof(PasswordBoxHelper) ,
            new FrameworkPropertyMetadata(string.Empty ,OnBoundPasswordChanged));

    public static readonly DependencyProperty BindPasswordProperty =
        DependencyProperty.RegisterAttached(
            "BindPassword" ,
            typeof(bool) ,
            typeof(PasswordBoxHelper) ,
            new PropertyMetadata(false ,OnBindPasswordChanged));

    public static string GetBoundPassword(DependencyObject obj) =>
        (string)obj.GetValue(BoundPasswordProperty);

    public static void SetBoundPassword(DependencyObject obj ,string value) =>
        obj.SetValue(BoundPasswordProperty ,value);

    public static bool GetBindPassword(DependencyObject obj) =>
        (bool)obj.GetValue(BindPasswordProperty);

    public static void SetBindPassword(DependencyObject obj ,bool value) =>
        obj.SetValue(BindPasswordProperty ,value);

    private static void OnBoundPasswordChanged(DependencyObject d ,DependencyPropertyChangedEventArgs e)
    {
        if(d is PasswordBox passwordBox)
        {
            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

            if(!string.Equals(passwordBox.Password ,e.NewValue as string))
            {
                passwordBox.Password = e.NewValue as string;
            }

            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }
    }

    private static void OnBindPasswordChanged(DependencyObject d ,DependencyPropertyChangedEventArgs e)
    {
        if(d is PasswordBox passwordBox)
        {
            if((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
            else
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            }
        }
    }

    private static void PasswordBox_PasswordChanged(object sender ,RoutedEventArgs e)
    {
        if(sender is PasswordBox passwordBox)
        {
            SetBoundPassword(passwordBox ,passwordBox.Password);
        }
    }
}
