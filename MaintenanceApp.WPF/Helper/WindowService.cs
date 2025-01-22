using MaintenanceApp.WPF.ViewModels;
using System;
using System.Windows;

namespace MaintenanceApp.WPF.Helper;

public class WindowService : IWindowService
{
    private readonly IServiceProvider _serviceProvider;

    public WindowService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void ShowWindow<TViewModel>(Action onClose = null) where TViewModel : BaseViewModel
    {
        var viewModel = _serviceProvider.GetService(typeof(TViewModel)) as TViewModel;
        var window = CreateWindow(viewModel ,onClose);
        window?.Show();
    }

    public void ShowDialog<TViewModel>(Action onClose = null) where TViewModel : BaseViewModel
    {
        var viewModel = _serviceProvider.GetService(typeof(TViewModel)) as TViewModel;
        var window = CreateWindow(viewModel ,onClose);
        window?.ShowDialog();
    }

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
        var windowTypeName = viewModelType.FullName.Replace("ViewModel" ,"Window");
        return Type.GetType(windowTypeName);
    }
}

