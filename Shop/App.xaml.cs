using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shop.Data;
using Shop.Services;
using Shop.ViewModels;
using Shop.Views;
using System;
using System.Windows;

namespace Shop
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // --- Настройка базы данных ---
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=shop.db"));

            // --- Регистрация сервисов ---
            services.AddScoped<AuthService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<CartViewModel>();

            // --- Регистрация ViewModels ---
            services.AddTransient<MainViewModel>();
            services.AddTransient<CatalogViewModel>();
            services.AddTransient<RegisterViewModel>();

            // --- Регистрация Views ---
            services.AddSingleton<MainWindow>();
            services.AddTransient<CatalogView>();
            services.AddTransient<CartView>();
            services.AddTransient<RegisterView>();

            _serviceProvider = services.BuildServiceProvider();

            // --- Применение миграций ---
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.MigrateAsync();
            }

            // --- Показываем окно регистрации ---
            ShowRegistrationWindow();
        }

        private void ShowRegistrationWindow()
        {
            var registerView = _serviceProvider.GetRequiredService<RegisterView>();
            var registerViewModel = _serviceProvider.GetRequiredService<RegisterViewModel>();

            registerView.DataContext = registerViewModel;

            // Подписываемся на событие успешной регистрации
            registerViewModel.RegistrationCompleted += (sender, args) =>
            {
                registerView.Close();
                ShowMainWindow();
            };

            registerView.ShowDialog(); // Открываем как диалоговое окно
        }

        private void ShowMainWindow()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}