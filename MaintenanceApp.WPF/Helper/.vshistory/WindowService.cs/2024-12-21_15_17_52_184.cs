using System;
using System.Windows;

namespace MaintenanceApp.WPF.Helper;

public class WindowService : IWindowService
{
    private readonly IContainerProvider _containerProvider;

    public WindowService(IContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
    }

    public void ShowWindow<TViewModel>(Action onClose = null) where TViewModel : BaseViewModel
    {
        var viewModel = _containerProvider.Resolve<TViewModel>();
        var window = CreateWindow(viewModel ,onClose);
        window?.Show();
    }

    public void ShowDialog<TViewModel>(Action onClose = null) where TViewModel : BaseViewModel
    {
        var viewModel = _containerProvider.Resolve<TViewModel>();
        var window = CreateWindow(viewModel ,onClose);
        window?.ShowDialog();
    }

    //public void ShowDialog<TViewModel>(Action onClose = null) where TViewModel : BaseViewModel
    //{
    //    // استخدم الـ containerProvider للحصول على المعاملات المطلوبة
    //    var databaseHelper = _containerProvider.Resolve<IDatabaseHelper>();
    //    var messageService = _containerProvider.Resolve<IMessageService>();

    //    // تمرير المعاملات إلى Unity Container باستخدام ParameterOverride
    //    var viewModel = _containerProvider.Resolve<TViewModel>();


    //    // إنشاء النافذة باستخدام الـ ViewModel
    //    var window = CreateWindow(viewModel ,onClose);
    //    window?.ShowDialog();
    //}

    private Window CreateWindow(BaseViewModel viewModel ,Action onClose)
    {
        if(viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        // البحث عن النافذة المناسبة بناءً على الـ ViewModel
        var windowType = FindWindowTypeForViewModel(viewModel.GetType());
        if(windowType == null)
            throw new InvalidOperationException($"No matching window found for {viewModel.GetType().Name}");

        var window = Activator.CreateInstance(windowType) as Window;
        if(window == null)
            throw new InvalidOperationException($"Unable to create instance of {windowType.Name}");

        window.DataContext = viewModel;

        if(onClose != null)
        {
            window.Closed += (_ ,_) => onClose();
        }

        return window;
    }

    private Type FindWindowTypeForViewModel(Type viewModelType)
    {
        var viewModelName = viewModelType.FullName;
        var windowName = viewModelName?.Replace("ViewModel" ,"Window");

        // التحقق إذا كانت النافذة موجودة
        var windowType = Type.GetType(windowName);
        if(windowType == null)
        {
            // إذا لم يتم العثور على النافذة، يمكنك معالجة الحالة أو استرجاع نافذة افتراضية
            throw new InvalidOperationException($"No matching window found for {viewModelType.Name}");
        }

        return windowType;
    }

}
