using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace MaintenanceApp.WPF.ViewModels;

public abstract class BaseViewModel : BindableBase, INotifyDataErrorInfo
{
    private readonly Dictionary<string ,List<string>> _errors = new();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public bool HasErrors => _errors.Count > 0;

    public IEnumerable GetErrors(string propertyName)
    {
        return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
    }

    protected void AddError(string propertyName ,string error)
    {
        if(!_errors.ContainsKey(propertyName))
            _errors[propertyName] = new List<string>();

        if(!_errors[propertyName].Contains(error))
        {
            _errors[propertyName].Add(error);
            ErrorsChanged?.Invoke(this ,new DataErrorsChangedEventArgs(propertyName));
        }
    }

    protected void ClearErrors(string propertyName)
    {
        if(_errors.Remove(propertyName))
            ErrorsChanged?.Invoke(this ,new DataErrorsChangedEventArgs(propertyName));
    }
}
