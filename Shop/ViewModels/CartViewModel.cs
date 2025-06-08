using Shop.Commands;
using Shop.Data;
using Shop.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class CartViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _db;

    private ObservableCollection<CartItem> _cartItems = new();
    public ObservableCollection<CartItem> CartItems
    {
        get => _cartItems;
        set
        {
            if (_cartItems != value)
            {
                _cartItems = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand RemoveFromCartCommand { get; }
    public ICommand IncreaseQuantityCommand { get; }
    public ICommand DecreaseQuantityCommand { get; }
    public ICommand CheckoutCommand { get; }

    public CartViewModel(AppDbContext db)
    {
        _db = db;

        RemoveFromCartCommand = new RelayCommand(async obj => await RemoveFromCart(obj));
        IncreaseQuantityCommand = new RelayCommand(async obj => await IncreaseQuantity(obj));
        DecreaseQuantityCommand = new RelayCommand(async obj => await DecreaseQuantity(obj));
        CheckoutCommand = new RelayCommand(Checkout);

        LoadCartItems();
    }

    private void LoadCartItems()
    {
        var items = _db.CartItems
            .Include(ci => ci.Product)
            .ToList();

        CartItems.Clear();
        foreach (var item in items)
            CartItems.Add(item);

        OnPropertyChanged(nameof(TotalPrice));
    }

    public async Task AddProduct(Product product)
    {
        var existing = CartItems.FirstOrDefault(ci => ci.ProductId == product.Id);
        if (existing != null)
        {
            existing.Quantity++;
            _db.CartItems.Update(existing);
        }
        else
        {
            var newItem = new CartItem { Product = product, ProductId = product.Id, Quantity = 1 };
            CartItems.Add(newItem);
            await _db.CartItems.AddAsync(newItem);
        }

        await _db.SaveChangesAsync();

        // Обновляем UI
        OnPropertyChanged(nameof(CartItems));
        OnPropertyChanged(nameof(TotalPrice));
    }

    private async Task RemoveFromCart(object? obj)
    {
        if (obj is CartItem item)
        {
            CartItems.Remove(item);
            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();

            OnPropertyChanged(nameof(CartItems));
            OnPropertyChanged(nameof(TotalPrice));
        }
    }

    private async Task IncreaseQuantity(object? obj)
    {
        if (obj is CartItem item)
        {
            item.Quantity++;
            _db.CartItems.Update(item);
            await _db.SaveChangesAsync();

            // Обновляем элемент в коллекции, чтобы UI обновился
            var index = CartItems.IndexOf(item);
            if (index >= 0)
            {
                CartItems.RemoveAt(index);
                CartItems.Insert(index, item);
            }

            OnPropertyChanged(nameof(TotalPrice));
        }
    }

    private async Task DecreaseQuantity(object? obj)
    {
        if (obj is CartItem item && item.Quantity > 1)
        {
            item.Quantity--;
            _db.CartItems.Update(item);
            await _db.SaveChangesAsync();

            var index = CartItems.IndexOf(item);
            if (index >= 0)
            {
                CartItems.RemoveAt(index);
                CartItems.Insert(index, item);
            }

            OnPropertyChanged(nameof(TotalPrice));
        }
    }

    private void Checkout(object? obj)
    {
        MessageBox.Show("Много хочешь", "Оформление заказа", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public decimal TotalPrice => CartItems.Sum(item => item.TotalPrice);

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
