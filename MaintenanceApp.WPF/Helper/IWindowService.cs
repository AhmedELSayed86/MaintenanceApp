using MaintenanceApp.WPF.ViewModels;
using System;

namespace MaintenanceApp.WPF.Helper;

public interface IWindowService
{
    void ShowWindow<TViewModel>(Action onClose = null) where TViewModel : BaseViewModel;
    void ShowDialog<TViewModel>(Action onClose = null) where TViewModel : BaseViewModel;
}