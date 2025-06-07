using Shop.Commands;
using Shop.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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

        NavigateCatalogCommand = new RelayCommand(_ => _navigationService.NavigateTo<Shop.Views.CatalogView>());
        NavigateCartCommand = new RelayCommand(_ => _navigationService.NavigateTo<Shop.Views.CartView>());

        // Подписка на смену текущего View
        _navigationService.CurrentViewChanged += (s, e) =>
        {
            CurrentPageView = _navigationService.CurrentView;
        };

        // Стартовая страница
        _navigationService.NavigateTo<Shop.Views.CatalogView>();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
