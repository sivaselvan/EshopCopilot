using EShop.ApiService.Models;

namespace EShop.ApiService.Services;

public class OrderService
{
    private readonly List<Order> _orders;
    private readonly object _syncRoot = new();
    private int _nextId;

    public OrderService()
    {
        _orders =
        [
            new Order
            {
                Id = 1,
                CustomerName = "Ava Thompson",
                OrderDate = new DateTime(2026, 9, 15),
                TotalAmount = 114.98m,
                Status = "Processing"
            },
            new Order
            {
                Id = 2,
                CustomerName = "Liam Martinez",
                OrderDate = new DateTime(2026, 9, 18),
                TotalAmount = 329.00m,
                Status = "Shipped"
            }
        ];

        _nextId = _orders.Count + 1;
    }

    public IEnumerable<Order> GetAll()
    {
        lock (_syncRoot)
        {
            return _orders.Select(CloneOrder).ToList();
        }
    }

    public Order? GetById(int id)
    {
        lock (_syncRoot)
        {
            var order = _orders.FirstOrDefault(order => order.Id == id);
            return order is not null ? CloneOrder(order) : null;
        }
    }

    public Order Create(Order order)
    {
        lock (_syncRoot)
        {
            var createdOrder = CloneOrder(order);
            createdOrder.Id = _nextId++;
            _orders.Add(createdOrder);
            return CloneOrder(createdOrder);
        }
    }

    public bool Update(int id, Order updatedOrder)
    {
        lock (_syncRoot)
        {
            var existingOrder = _orders.FirstOrDefault(order => order.Id == id);
            if (existingOrder is null)
            {
                return false;
            }

            existingOrder.CustomerName = updatedOrder.CustomerName;
            existingOrder.OrderDate = updatedOrder.OrderDate;
            existingOrder.TotalAmount = updatedOrder.TotalAmount;
            existingOrder.Status = updatedOrder.Status;
            existingOrder.Items = updatedOrder.Items.Select(CloneOrderItem).ToList();

            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (_syncRoot)
        {
            var existingOrder = _orders.FirstOrDefault(order => order.Id == id);
            if (existingOrder is null)
            {
                return false;
            }

            _orders.Remove(existingOrder);
            return true;
        }
    }

    private static Order CloneOrder(Order order) => new()
    {
        Id = order.Id,
        CustomerName = order.CustomerName,
        OrderDate = order.OrderDate,
        TotalAmount = order.TotalAmount,
        Status = order.Status,
        Items = order.Items.Select(CloneOrderItem).ToList()
    };

    private static OrderItem CloneOrderItem(OrderItem item) => new()
    {
        ProductId = item.ProductId,
        ProductName = item.ProductName,
        UnitPrice = item.UnitPrice,
        Quantity = item.Quantity
    };
}
