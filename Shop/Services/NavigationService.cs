using Microsoft.Extensions.DependencyInjection;
using Shop.Services;
using Shop.ViewModels;
using System;
using System.Windows.Controls;
using Shop.Views;
using System.Windows;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public event EventHandler CurrentViewChanged;

    private object _currentView;
    public object CurrentView
    {
        get => _currentView;
        private set
        {
            _currentView = value;
            OnCurrentViewChanged();
        }
    }

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Навигация по типам View и ViewModel
    /// </summary>
    public void NavigateTo<TView, TViewModel>()
        where TView : UserControl
        where TViewModel : class
    {
        var view = _serviceProvider.GetRequiredService<TView>();
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

        view.DataContext = viewModel;
        CurrentView = view;
    }

    /// <summary>
    /// Навигация только по ViewModel (для случаев, когда View привязан через DataTemplate)
    /// </summary>
    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        CurrentView = viewModel;
    }

    /// <summary>
    /// Навигация по строковому ключу (например, из меню)
    /// </summary>
    public void NavigateTo(string viewName)
    {
        switch (viewName)
        {
            case "Catalog":
                NavigateTo<CatalogView, CatalogViewModel>();
                break;
            case "Cart":
                NavigateTo<CartView, CartViewModel>();
                break;
            /*case "Login":
                NavigateTo<LoginView, LoginViewModel>();
                break;*/
            default:
                throw new ArgumentException($"Неизвестный View: {viewName}", nameof(viewName));
        }
    }

    /// <summary>
    /// Метод для принудительного обновления текущего представления
    /// </summary>
    public void Refresh()
    {
        if (CurrentView is FrameworkElement element && element.DataContext is object context)
        {
            // Можно пересоздать DataContext или вызвать обновление данных
            var newContext = _serviceProvider.GetService(context.GetType());
            if (newContext != null)
            {
                element.DataContext = null;
                element.DataContext = newContext;
            }
        }
    }

    protected virtual void OnCurrentViewChanged()
    {
        CurrentViewChanged?.Invoke(this, EventArgs.Empty);
    }
}