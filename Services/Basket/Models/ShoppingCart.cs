using System;

namespace Basket.Models;

public class ShoppingCart
{
    public string UserName { get; set; } = string.Empty;
    public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
    public decimal TotalPrice => Items.Sum(item => item.Quantity * item.Price);
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}