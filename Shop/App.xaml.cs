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

            // Регистрация БД
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=shop.db"));

            // Регистрация сервисов
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddTransient<ShopDbService>();

            // Регистрация ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<CatalogViewModel>();
            services.AddTransient<CartViewModel>();

            // Регистрация Views
            services.AddSingleton<MainWindow>();
            services.AddTransient<CatalogView>();
            services.AddTransient<CartView>();

            _serviceProvider = services.BuildServiceProvider();

            // Инициализация БД
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.MigrateAsync();
            }

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}