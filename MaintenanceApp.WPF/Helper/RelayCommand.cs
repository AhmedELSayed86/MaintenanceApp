﻿using System;
using System.Windows.Input;

namespace MaintenanceApp.WPF.Helper;

/// <summary>
/// تنفيذ واجهة ICommand للأوامر.
/// </summary>
public class RelayCommand(Action<object> execute ,Func<object ,bool> canExecute = null) : ICommand
{ 
    private readonly Action<object> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

    public bool CanExecute(object parameter)
    {
        return canExecute == null || canExecute(parameter);
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
}