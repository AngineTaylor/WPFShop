using Shop.Models;

public class CartItem
{
    public int Id { get; set; }

    public Product Product { get; set; } = null!;
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice => Product.Price * Quantity;

    // Связь с заказом
    public int? OrderId { get; set; }  // может быть null, пока товар в корзине
    public Order? Order { get; set; }
}
