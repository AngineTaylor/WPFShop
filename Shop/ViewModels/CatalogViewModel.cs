using Shop.Data;
using Shop.Models;
using Shop.ViewModels;
using Shop.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using System.Linq;

namespace Shop.ViewModels
{
    public class CatalogViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _dbContext;
        private readonly CartViewModel _cartViewModel;

        public CatalogViewModel(AppDbContext dbContext, CartViewModel cartViewModel)
        {
            _dbContext = dbContext;
            _cartViewModel = cartViewModel;

            AddToCartCommand = new RelayCommand(AddToCart);
            LoadProducts();
        }

        public ObservableCollection<Product> Products { get; set; } = new();

        public ICommand AddToCartCommand { get; }

        private void AddToCart(object? parameter)
        {
            if (parameter is Product product)
            {
                _cartViewModel.AddProduct(product);
            }
        }

        private async void LoadProducts()
        {
            var products = await _dbContext.Products.ToListAsync();
            Products = new ObservableCollection<Product>(products);
            OnPropertyChanged(nameof(Products));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
