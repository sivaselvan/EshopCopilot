namespace EShop.ApiService.Models;

public class ShoppingCart
{
    public string CustomerName { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = [];
    public decimal TotalAmount => Items.Sum(item => item.LineTotal);
}
