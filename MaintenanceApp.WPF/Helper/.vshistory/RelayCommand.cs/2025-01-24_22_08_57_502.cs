using System;
using System.Windows.Input;

namespace MaintenanceApp.WPF.Helper;

/// <summary>
/// تنفيذ واجهة ICommand للأوامر.
/// </summary>
public class RelayCommand(Action execute ,Func<bool> canExecute = null) : ICommand
{
    private readonly Action _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private readonly Func<bool> _canExecute = canExecute;

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute();
    }

    public void Execute(object parameter)
    {
        _execute();
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}