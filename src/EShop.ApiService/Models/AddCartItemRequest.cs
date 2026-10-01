namespace EShop.ApiService.Models;

public class AddCartItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
