using System;

namespace Shop.Services
{
    public interface INavigationService
    {
        event EventHandler? CurrentViewChanged;

        object? CurrentView { get; }

        void NavigateTo<TView>() where TView : class;
    }
}
