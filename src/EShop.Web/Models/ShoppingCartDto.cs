namespace EShop.Web.Models;

public sealed class ShoppingCartDto
{
    public string CustomerName { get; set; } = string.Empty;
    public List<CartItemDto> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
}

public sealed class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

public sealed class AddCartItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
