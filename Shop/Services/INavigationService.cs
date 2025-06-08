using System;
using System.Windows.Controls;

namespace Shop.Services
{
    public interface INavigationService
    {
        event EventHandler? CurrentViewChanged;
        object? CurrentView { get; }
        void NavigateTo<TView, TViewModel>()
            where TView : UserControl
            where TViewModel : class;
    }


}
