using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MaintenanceApp.WPF.ViewModels;

/// <summary>
/// هذا هو النموذج الأساسي (BaseViewModel) الذي يدعم:
/// - التحقق من الأخطاء (INotifyDataErrorInfo).
/// - تحديث خصائص الواجهة (INotifyPropertyChanged).
/// </summary>
public abstract class BaseViewModel : INotifyDataErrorInfo, INotifyPropertyChanged
{
    // قائمة لتخزين الأخطاء المرتبطة بالخصائص
    private readonly Dictionary<string ,List<string>> _errors = new();

    /// <summary>
    /// حدث يتم إطلاقه عند تغيير الأخطاء المرتبطة بخصائص معينة.
    /// </summary>
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    /// <summary>
    /// خاصية تحدد ما إذا كان النموذج يحتوي على أخطاء.
    /// </summary>
    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// الحصول على الأخطاء المرتبطة بخاصية معينة.
    /// </summary>
    /// <param name="propertyName">اسم الخاصية.</param>
    /// <returns>قائمة بالأخطاء.</returns>
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

    // خاصية لتحديد إذا كان النموذج مشغولاً (يمكن استخدامها لعرض مؤشر انتظار).
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy ,value);
    }

    /// <summary>
    /// حدث يتم إطلاقه عند تغيير أي خاصية.
    /// </summary>
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
        return true;
    }

    /// <summary>
    /// التحقق من صحة خاصية معينة وإضافة أو إزالة الأخطاء بناءً على القواعد.
    /// </summary>
    /// <param name="propertyName">اسم الخاصية.</param>
    /// <param name="validationRule">دالة للتحقق من صحة الخاصية.</param>
    /// <param name="errorMessage">الرسالة التي تظهر عند وجود خطأ.</param>
    protected void ValidateProperty(string propertyName ,Func<bool> validationRule ,string errorMessage)
    {
        if(validationRule())
            AddError(propertyName ,errorMessage);
        else
            ClearErrors(propertyName);
    }
}
