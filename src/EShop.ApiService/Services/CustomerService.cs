using EShop.ApiService.Models;

namespace EShop.ApiService.Services;

public class CustomerService
{
    private readonly List<Customer> _customers =
    [
        new Customer
        {
            Id = Guid.Parse("5f45aad3-53b1-4b20-a416-6d8e7d91d87e"),
            FirstName = "Ava",
            LastName = "Thompson",
            Email = "ava.thompson@example.com"
        },
        new Customer
        {
            Id = Guid.Parse("b6ec076a-312a-4099-9d1c-8bf1bb7fcae0"),
            FirstName = "Liam",
            LastName = "Martinez",
            Email = "liam.martinez@example.com"
        }
    ];

    public IEnumerable<Customer> GetAll() => _customers;

    public Customer? GetById(Guid id) => _customers.FirstOrDefault(customer => customer.Id == id);

    public Customer Create(Customer customer)
    {
        customer.Id = Guid.NewGuid();
        _customers.Add(customer);
        return customer;
    }

    public bool Update(Guid id, Customer updatedCustomer)
    {
        var existingCustomer = GetById(id);
        if (existingCustomer is null)
        {
            return false;
        }

        existingCustomer.FirstName = updatedCustomer.FirstName;
        existingCustomer.LastName = updatedCustomer.LastName;
        existingCustomer.Email = updatedCustomer.Email;

        return true;
    }

    public bool Delete(Guid id)
    {
        var existingCustomer = GetById(id);
        if (existingCustomer is null)
        {
            return false;
        }

        _customers.Remove(existingCustomer);
        return true;
    }
}
