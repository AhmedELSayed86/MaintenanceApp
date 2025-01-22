using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MaintenanceApp.WPF.Behaviors;

public static class DataGridBehaviors 
{
    public static readonly DependencyProperty SelectionChangedCommandProperty =
        DependencyProperty.RegisterAttached(
            "SelectionChangedCommand" ,
            typeof(ICommand) ,
            typeof(DataGridBehaviors) ,
            new PropertyMetadata(null ,OnSelectionChangedCommandChanged));

    public static ICommand GetSelectionChangedCommand(DependencyObject obj) =>
        (ICommand)obj.GetValue(SelectionChangedCommandProperty);

    public static void SetSelectionChangedCommand(DependencyObject obj ,ICommand value) =>
        obj.SetValue(SelectionChangedCommandProperty ,value);

    private static void OnSelectionChangedCommandChanged(DependencyObject d ,DependencyPropertyChangedEventArgs e)
    {
        if(d is DataGrid dataGrid)
        {
            dataGrid.SelectionChanged -= DataGrid_SelectionChanged;

            if(e.NewValue != null)
            {
                dataGrid.SelectionChanged += DataGrid_SelectionChanged;
            }
        }
    }

    private static void DataGrid_SelectionChanged(object sender ,SelectionChangedEventArgs e)
    {
        if(sender is DataGrid dataGrid)
        {
            var command = GetSelectionChangedCommand(dataGrid);
            if(command != null && command.CanExecute(dataGrid.SelectedItem))
            {
                command.Execute(dataGrid.SelectedItem);
            }
        }
    }
}
