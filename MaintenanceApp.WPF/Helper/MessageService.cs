using System.Windows;

namespace MaintenanceApp.WPF.Helper;

public class MessageService : IMessageService
{
    public void ShowInfo(string message) => MessageBox.Show(message ,"Information" ,MessageBoxButton.OK ,MessageBoxImage.Information);
    public void ShowError(string message) => MessageBox.Show(message ,"Error" ,MessageBoxButton.OK ,MessageBoxImage.Error);
}
 