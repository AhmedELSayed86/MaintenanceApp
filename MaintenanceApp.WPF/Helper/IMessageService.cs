namespace MaintenanceApp.WPF.Helper;

public interface IMessageService
{
    void ShowInfo(string message);
    void ShowError(string message);
}
