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
        CheckoutCommand = new RelayCommand(async obj => await Checkout());

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

            RefreshItemInCollection(item);
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

            RefreshItemInCollection(item);
            OnPropertyChanged(nameof(TotalPrice));
        }
    }

    private void RefreshItemInCollection(CartItem item)
    {
        var index = CartItems.IndexOf(item);
        if (index >= 0)
        {
            CartItems.RemoveAt(index);
            CartItems.Insert(index, item);
        }
    }

    private async Task Checkout()
    {
        if (CartItems.Count == 0)
        {
            MessageBox.Show("Корзина пуста");
            return;
        }

        var order = new Order
        {
            UserId = "CurrentUserId", // Здесь подставь реальный UserId из контекста пользователя
            Status = "Новый",
            TotalPrice = this.TotalPrice,
            Items = CartItems.Select(ci => new CartItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                // Не нужно указывать OrderId, EF это сделает автоматически
            }).ToList()
        };

        // Добавляем заказ с элементами
        _db.Orders.Add(order);

        // Удаляем текущие элементы из корзины (т.к. они теперь в заказе)
        _db.CartItems.RemoveRange(CartItems);
        CartItems.Clear();

        await _db.SaveChangesAsync();

        OnPropertyChanged(nameof(CartItems));
        OnPropertyChanged(nameof(TotalPrice));

        MessageBox.Show("Заказ оформлен!");
    }


    public decimal TotalPrice => CartItems.Sum(item => item.Product?.Price * item.Quantity ?? 0);

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
