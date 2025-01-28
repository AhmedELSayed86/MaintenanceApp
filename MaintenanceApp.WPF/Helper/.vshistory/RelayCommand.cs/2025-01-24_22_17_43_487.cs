using System;
using System.Windows.Input;

namespace MaintenanceApp.WPF.Helper;

/// <summary>
/// تنفيذ واجهة ICommand للأوامر.
/// </summary>
public class RelayCommand(Action execute ,Func<bool> canExecute = null) : ICommand
{ 
    private readonly Action<object> _execute;
    private readonly Func<object ,bool> _canExecute;

    public RelayCommand(Action<object> execute ,Func<object ,bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        _execute(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }








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