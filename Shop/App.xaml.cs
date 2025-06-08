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

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=shop.db"));

            // Регистрация сервисов и моделей
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddTransient<ShopDbService>();
            services.AddSingleton<CartViewModel>();

            // Регистрация ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<CatalogViewModel>();

            // Регистрация Views
            services.AddSingleton<MainWindow>();
            services.AddTransient<CatalogView>();
            services.AddTransient<CartView>();

            _serviceProvider = services.BuildServiceProvider();

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