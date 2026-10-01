using EShop.ApiService.Models;

namespace EShop.ApiService.Services;

public class ShoppingCartService
{
    private readonly List<ShoppingCart> _carts = [];
    private readonly object _syncRoot = new();
    private readonly ProductService _productService;
    private readonly OrderService _orderService;

    public ShoppingCartService(ProductService productService, OrderService orderService)
    {
        _productService = productService;
        _orderService = orderService;
    }

    public ShoppingCart? GetByCustomerName(string customerName)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            return null;
        }

        lock (_syncRoot)
        {
            var cart = GetByCustomerNameCore(customerName);
            return cart is not null ? CloneCart(cart) : null;
        }
    }

    public ShoppingCart? AddItem(string customerName, int productId, int quantity)
    {
        if (string.IsNullOrWhiteSpace(customerName) || quantity <= 0)
        {
            return null;
        }

        var product = _productService.GetById(productId);
        if (product is null)
        {
            return null;
        }

        lock (_syncRoot)
        {
            var cart = GetByCustomerNameCore(customerName);
            if (cart is null)
            {
                cart = new ShoppingCart { CustomerName = customerName.Trim() };
                _carts.Add(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem is not null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = quantity
                });
            }

            return CloneCart(cart);
        }
    }

    public bool RemoveItem(string customerName, int productId)
    {
        lock (_syncRoot)
        {
            var cart = GetByCustomerNameCore(customerName);
            if (cart is null)
            {
                return false;
            }

            var item = cart.Items.FirstOrDefault(cartItem => cartItem.ProductId == productId);
            if (item is null)
            {
                return false;
            }

            cart.Items.Remove(item);
            return true;
        }
    }

    public Order? Checkout(string customerName)
    {
        lock (_syncRoot)
        {
            var cart = GetByCustomerNameCore(customerName);
            if (cart is null || cart.Items.Count == 0)
            {
                return null;
            }

            var order = _orderService.Create(new Order
            {
                CustomerName = cart.CustomerName,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cart.TotalAmount,
                Status = "Processing",
                Items = cart.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                }).ToList()
            });

            _carts.Remove(cart);
            return order;
        }
    }

    private ShoppingCart? GetByCustomerNameCore(string customerName) =>
        _carts.FirstOrDefault(cart =>
            string.Equals(cart.CustomerName, customerName.Trim(), StringComparison.OrdinalIgnoreCase));

    private static ShoppingCart CloneCart(ShoppingCart cart) => new()
    {
        CustomerName = cart.CustomerName,
        Items = cart.Items.Select(item => new CartItem
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity
        }).ToList()
    };
}
