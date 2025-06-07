using Shop.Models;

public class CartItem
{
    public int Id { get; set; } // Это и есть первичный ключ

    public Product? Product { get; set; }
    public int Quantity { get; set; }

    public decimal TotalPrice => Product?.Price * Quantity ?? 0m;
}