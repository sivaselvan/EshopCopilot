using EShop.ApiService.Models;
using EShop.ApiService.Services;

namespace EShop.ApiService.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapGet("/", (OrderService orderService) =>
            Results.Ok(orderService.GetAll()));

        group.MapGet("/{id:int}", (int id, OrderService orderService) =>
        {
            var order = orderService.GetById(id);
            return order is not null ? Results.Ok(order) : Results.NotFound();
        });

        group.MapPost("/", (Order order, OrderService orderService) =>
        {
            var created = orderService.Create(order);
            return Results.Created($"/api/orders/{created.Id}", created);
        });

        group.MapPut("/{id:int}", (int id, Order order, OrderService orderService) =>
        {
            var updated = orderService.Update(id, order);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", (int id, OrderService orderService) =>
        {
            var deleted = orderService.Delete(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
