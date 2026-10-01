using EShop.ApiService.Models;

namespace EShop.ApiService.Services;

public class ProductService
{
    private readonly List<Product> _products;
    private int _nextId;

    public ProductService()
    {
        _products =
        [
            new Product
            {
                Id = 1,
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse with adjustable DPI settings.",
                Price = 24.99m,
                ImageUri = "https://example.com/images/wireless-mouse.png"
            },
            new Product
            {
                Id = 2,
                Name = "Mechanical Keyboard",
                Description = "RGB backlit mechanical keyboard with hot-swappable switches.",
                Price = 89.99m,
                ImageUri = "https://example.com/images/mechanical-keyboard.png"
            },
            new Product
            {
                Id = 3,
                Name = "27-inch 4K Monitor",
                Description = "Ultra HD monitor with HDR support and slim bezels.",
                Price = 329.00m,
                ImageUri = "https://example.com/images/4k-monitor.png"
            },
            new Product
            {
                Id = 4,
                Name = "USB-C Docking Station",
                Description = "11-in-1 docking station with dual HDMI and 100W power delivery.",
                Price = 59.50m,
                ImageUri = "https://example.com/images/usb-c-dock.png"
            },
            new Product
            {
                Id = 5,
                Name = "Noise-Cancelling Headphones",
                Description = "Over-ear headphones with active noise cancellation and 30-hour battery life.",
                Price = 149.99m,
                ImageUri = "https://example.com/images/headphones.png"
            }
        ];

        _nextId = _products.Count + 1;
    }

    public IEnumerable<Product> GetAll() => _products;

    public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

    public Product Create(Product product)
    {
        product.Id = _nextId++;
        _products.Add(product);
        return product;
    }

    public bool Update(int id, Product updatedProduct)
    {
        var existingProduct = GetById(id);
        if (existingProduct is null)
        {
            return false;
        }

        existingProduct.Name = updatedProduct.Name;
        existingProduct.Description = updatedProduct.Description;
        existingProduct.Price = updatedProduct.Price;
        existingProduct.ImageUri = updatedProduct.ImageUri;

        return true;
    }

    public bool Delete(int id)
    {
        var existingProduct = GetById(id);
        if (existingProduct is null)
        {
            return false;
        }

        _products.Remove(existingProduct);
        return true;
    }
}
