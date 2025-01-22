using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MaintenanceApp.WPF.ViewModels;

public abstract class BaseViewModel : INotifyDataErrorInfo, INotifyPropertyChanged
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
            OnErrorsChanged(propertyName);
        }
    }

    protected void ClearErrors(string propertyName)
    {
        if(_errors.Remove(propertyName))
            OnErrorsChanged(propertyName);
    }

    protected void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this ,new DataErrorsChangedEventArgs(propertyName));
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy ,value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this ,new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T backingField ,T value ,[CallerMemberName] string propertyName = null)
    {
        if(EqualityComparer<T>.Default.Equals(backingField ,value))
            return false;

        backingField = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void ValidateProperty(string propertyName ,Func<bool> validationRule ,string errorMessage)
    {
        if(validationRule())
            AddError(propertyName ,errorMessage);
        else
            ClearErrors(propertyName);
    }
}
