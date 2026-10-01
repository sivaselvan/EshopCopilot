using EShop.ApiService.Models;

namespace EShop.ApiService.Services;

public class OrderService
{
    private readonly List<Order> _orders;
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

    public IEnumerable<Order> GetAll() => _orders;

    public Order? GetById(int id) => _orders.FirstOrDefault(order => order.Id == id);

    public Order Create(Order order)
    {
        order.Id = _nextId++;
        _orders.Add(order);
        return order;
    }

    public bool Update(int id, Order updatedOrder)
    {
        var existingOrder = GetById(id);
        if (existingOrder is null)
        {
            return false;
        }

        existingOrder.CustomerName = updatedOrder.CustomerName;
        existingOrder.OrderDate = updatedOrder.OrderDate;
        existingOrder.TotalAmount = updatedOrder.TotalAmount;
        existingOrder.Status = updatedOrder.Status;

        return true;
    }

    public bool Delete(int id)
    {
        var existingOrder = GetById(id);
        if (existingOrder is null)
        {
            return false;
        }

        _orders.Remove(existingOrder);
        return true;
    }
}
