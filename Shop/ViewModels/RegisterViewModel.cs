using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Shop.Commands;
using Shop.Views;
using Microsoft.Extensions.DependencyInjection; // Добавлено для IServiceProvider
using Shop.Services;

namespace Shop.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        // Событие успешной регистрации
        public event EventHandler RegistrationCompleted;

        private string _firstName;
        private string _lastName;
        private string _email;
        private string _password;

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand RegisterCommand { get; }

        // Внедряем IServiceProvider через конструктор
        private readonly IServiceProvider _serviceProvider;

        public RegisterViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _authService = serviceProvider.GetRequiredService<AuthService>();
            RegisterCommand = new RelayCommand(Register, CanRegister);
        }

        private bool CanRegister(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password);
        }
        private readonly AuthService _authService;

        private void Register(object parameter)
        {
            try
            {
                bool success = _authService.RegisterUser(FirstName, LastName, Email, Password);

                if (success)
                {
                    // Получаем MainViewModel через DI
                    var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();

                    // Создаём MainWindow и передаём ViewModel
                    var mainWindow = new MainWindow(mainViewModel); // Предполагается, что у MainWindow есть конструктор без параметров
                    mainWindow.DataContext = mainViewModel;

                    // Устанавливаем главное окно приложения
                    Application.Current.MainWindow = mainWindow;

                    mainWindow.Show();

                    // Закрываем текущее окно
                    var currentWindow = Application.Current.Windows
                        .OfType<Window>()
                        .FirstOrDefault(w => w.DataContext == this);

                    currentWindow?.Close();
                }
                else
                {
                    MessageBox.Show("Пользователь с таким email уже существует!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}