using EShop.ApiService.Models;
using EShop.ApiService.Services;

namespace EShop.ApiService.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers").WithTags("Customers");

        group.MapGet("/", (CustomerService customerService) =>
            Results.Ok(customerService.GetAll()));

        group.MapGet("/{id:guid}", (Guid id, CustomerService customerService) =>
        {
            var customer = customerService.GetById(id);
            return customer is not null ? Results.Ok(customer) : Results.NotFound();
        });

        group.MapPost("/", (Customer customer, CustomerService customerService) =>
        {
            var created = customerService.Create(customer);
            return Results.Created($"/api/customers/{created.Id}", created);
        });

        group.MapPut("/{id:guid}", (Guid id, Customer customer, CustomerService customerService) =>
        {
            var updated = customerService.Update(id, customer);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:guid}", (Guid id, CustomerService customerService) =>
        {
            var deleted = customerService.Delete(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
