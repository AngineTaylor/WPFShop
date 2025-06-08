using Shop.Commands;
using Shop.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Shop.ViewModels;
using Shop.Views;

public class MainViewModel : INotifyPropertyChanged
{
    private object _currentPageView;
    public object CurrentPageView
    {
        get => _currentPageView;
        set
        {
            if (_currentPageView != value)
            {
                _currentPageView = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand NavigateCatalogCommand { get; }
    public ICommand NavigateCartCommand { get; }

    private readonly INavigationService _navigationService;

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        // Инициализация команд
        NavigateCatalogCommand = new RelayCommand(OnNavigateToCatalog);
        NavigateCartCommand = new RelayCommand(OnNavigateToCart);

        // Подписка на обновление представления
        _navigationService.CurrentViewChanged += OnCurrentViewChanged;

        // Начальная навигация
        _navigationService.NavigateTo<CatalogView, CatalogViewModel>();
    }

    private void OnNavigateToCatalog(object? param)
    {
        _navigationService.NavigateTo<CatalogView, CatalogViewModel>();
    }

    private void OnNavigateToCart(object? param)
    {
        _navigationService.NavigateTo<CartView, CartViewModel>();
    }

    private void OnCurrentViewChanged(object? sender, EventArgs e)
    {
        CurrentPageView = _navigationService.CurrentView;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}