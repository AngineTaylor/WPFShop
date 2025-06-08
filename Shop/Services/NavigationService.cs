using Microsoft.Extensions.DependencyInjection;
using Shop.Services;
using System.Windows.Controls;

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
            CurrentViewChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<TView, TViewModel>()
        where TView : UserControl
        where TViewModel : class
    {
        var view = _serviceProvider.GetService(typeof(TView)) as UserControl;
        var viewModel = _serviceProvider.GetService(typeof(TViewModel));
        if (view == null || viewModel == null)
            throw new InvalidOperationException("View или ViewModel не зарегистрированы в DI");

        view.DataContext = viewModel;
        CurrentView = view;
    }
    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        var viewModel = _serviceProvider.GetService<TViewModel>();
        if (viewModel == null)
            throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} не зарегистрирован в DI.");

        CurrentView = viewModel;
    }

}
