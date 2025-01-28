using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MaintenanceApp.WPF.ViewModels;

/// <summary>
/// النموذج الأساسي (BaseViewModel) الذي يدعم:
/// - تحديث الخصائص (INotifyPropertyChanged).
/// - التحقق من الأخطاء (INotifyDataErrorInfo).
/// - تسجيل الأخطاء باستخدام NLog.
/// - دعم الأوامر (ICommand).
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
    // Logger مشترك لجميع الـ ViewModels
    protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    // قائمة لتخزين الأخطاء المرتبطة بالخصائص
    private readonly Dictionary<string ,List<string>> _errors = new();

    // حدث يتم إطلاقه عند تغيير الأخطاء المرتبطة بخصائص معينة
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    // خاصية تحدد ما إذا كان النموذج يحتوي على أخطاء
    public bool HasErrors => _errors.Count > 0;

    // خاصية لتحديد إذا كان النموذج مشغولاً (يمكن استخدامها لعرض مؤشر انتظار)
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy ,value);
    }

    // حدث يتم إطلاقه عند تغيير أي خاصية
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// إطلاق حدث `PropertyChanged` عند تغيير قيمة خاصية.
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this ,new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// تعيين قيمة خاصية مع التحقق من التغيير وإطلاق الحدث إذا لزم الأمر.
    /// </summary>
    protected bool SetProperty<T>(ref T backingField ,T value ,[CallerMemberName] string propertyName = null)
    {
        if(EqualityComparer<T>.Default.Equals(backingField ,value))
            return false;

        backingField = value;
        OnPropertyChanged(propertyName);
        ValidateProperty(propertyName ,value); // التحقق من صحة الخاصية تلقائيًا
        return true;
    }

    /// <summary>
    /// الحصول على الأخطاء المرتبطة بخاصية معينة.
    /// </summary>
    public IEnumerable GetErrors(string propertyName)
    {
        return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
    }

    /// <summary>
    /// إضافة خطأ جديد إلى خاصية معينة.
    /// </summary>
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

    /// <summary>
    /// إزالة جميع الأخطاء المرتبطة بخاصية معينة.
    /// </summary>
    protected void ClearErrors(string propertyName)
    {
        if(_errors.Remove(propertyName))
            OnErrorsChanged(propertyName);
    }

    /// <summary>
    /// إطلاق حدث `ErrorsChanged` عندما تتغير الأخطاء.
    /// </summary>
    protected void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this ,new DataErrorsChangedEventArgs(propertyName));
    }

    /// <summary>
    /// التحقق من صحة خاصية معينة بناءً على القواعد المحددة.
    /// </summary>
    protected void ValidateProperty<T>(string propertyName ,T value)
    {
        // يمكنك إضافة قواعد التحقق هنا
        if(value is string strValue && string.IsNullOrEmpty(strValue))
        {
            AddError(propertyName ,$"{propertyName} مطلوب.");
        }
        else
        {
            ClearErrors(propertyName);
        }
    }

    /// <summary>
    /// تسجيل رسالة معلومات (Info) باستخدام NLog.
    /// </summary>
    protected void LogInfo(string message)
    {
        Logger.Info(message);
    }

    /// <summary>
    /// تسجيل رسالة تحذير (Warning) باستخدام NLog.
    /// </summary>
    protected void LogWarning(string message)
    {
        Logger.Warn(message);
    }

    /// <summary>
    /// تسجيل رسالة خطأ (Error) باستخدام NLog.
    /// </summary>
    protected void LogError(string message ,Exception ex = null)
    {
        if(ex == null)
            Logger.Error(message);
        else
            Logger.Error(ex ,message);
    }

    /// <summary>
    /// إنشاء أمر (Command) جديد.
    /// </summary>
    protected ICommand CreateCommand(Action execute ,Func<bool> canExecute = null)
    {
        return new RelayCommand(execute ,canExecute);
    }
}

/// <summary>
/// تنفيذ واجهة ICommand للأوامر.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public RelayCommand(Action execute ,Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

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
