using Shop.Commands;
using Shop.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Shop.ViewModels;

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
        _navigationService = navigationService;

        NavigateCatalogCommand = new RelayCommand(_ =>
            _navigationService.NavigateTo<Shop.Views.CatalogView, CatalogViewModel>());

        NavigateCartCommand = new RelayCommand(_ =>
            _navigationService.NavigateTo<Shop.Views.CartView, CartViewModel>());

        _navigationService.CurrentViewChanged += (s, e) =>
        {
            CurrentPageView = _navigationService.CurrentView;
        };

        _navigationService.NavigateTo<Shop.Views.CatalogView, CatalogViewModel>();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
