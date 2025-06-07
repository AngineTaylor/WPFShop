using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Shop.Models;
using Shop.Commands;

namespace Shop.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CartItem> CartItems { get; set; }

        public ICommand RemoveFromCartCommand { get; }
        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand CheckoutCommand { get; }

        public CartViewModel()
        {
            // Пример данных
            CartItems = new ObservableCollection<CartItem>
            {
                new CartItem
                {
                    Product = new Product { Id = 1, Name = "Смартфон", Price = 29990 },
                    Quantity = 2
                },
                new CartItem
                {
                    Product = new Product { Id = 3, Name = "Научная фантастика", Price = 890 },
                    Quantity = 1
                }
            };

            RemoveFromCartCommand = new RelayCommand(RemoveFromCart);
            IncreaseQuantityCommand = new RelayCommand(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(DecreaseQuantity);
            CheckoutCommand = new RelayCommand(Checkout);
        }

        private void RemoveFromCart(object? obj)
        {
            if (obj is CartItem item && CartItems.Contains(item))
            {
                CartItems.Remove(item);
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private void IncreaseQuantity(object? obj)
        {
            if (obj is CartItem item)
            {
                item.Quantity++;
                OnPropertyChanged(nameof(item.TotalPrice));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private void DecreaseQuantity(object? obj)
        {
            if (obj is CartItem item && item.Quantity > 1)
            {
                item.Quantity--;
                OnPropertyChanged(nameof(item.TotalPrice));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private void Checkout(object? obj)
        {
            // Здесь можно реализовать переход к оформлению заказа
            // или вызов сервиса создания заказа
        }

        public decimal TotalPrice => CartItems.Sum(item => item.TotalPrice);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}