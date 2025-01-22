using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace MaintenanceApp.WPF.Behaviors;

public class DataGridSelectedItemsBehavior : Behavior<DataGrid>
{
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register(
            nameof(SelectedItems) ,
            typeof(IList) ,
            typeof(DataGridSelectedItemsBehavior) ,
            new FrameworkPropertyMetadata(null ,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault ,OnSelectedItemsChanged));

    public IList SelectedItems
    {
        get => (IList)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty ,value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.SelectionChanged += OnSelectionChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.SelectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged(object sender ,SelectionChangedEventArgs e)
    {
        SelectedItems = AssociatedObject.SelectedItems;
    }

    private static void OnSelectedItemsChanged(DependencyObject d ,DependencyPropertyChangedEventArgs e)
    {
        // يمكنك إضافة منطق إضافي هنا إذا لزم الأمر
    }
}
