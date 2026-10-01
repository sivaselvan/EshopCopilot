using EShop.ApiService.Models;
using EShop.ApiService.Services;

namespace EShop.ApiService.Endpoints;

public static class ShoppingCartEndpoints
{
    public static IEndpointRouteBuilder MapShoppingCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/carts").WithTags("Shopping Carts");

        group.MapGet("/{customerName}", (string customerName, ShoppingCartService shoppingCartService) =>
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                return Results.BadRequest("Customer name is required.");
            }

            var cart = shoppingCartService.GetByCustomerName(customerName);
            return cart is not null ? Results.Ok(cart) : Results.NotFound();
        });

        group.MapPost("/{customerName}/items", (string customerName, AddCartItemRequest request, ShoppingCartService shoppingCartService) =>
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                return Results.BadRequest("Customer name is required.");
            }

            if (request.Quantity <= 0)
            {
                return Results.BadRequest("Quantity must be greater than zero.");
            }

            var cart = shoppingCartService.AddItem(customerName, request.ProductId, request.Quantity);
            return cart is not null ? Results.Ok(cart) : Results.NotFound();
        });

        group.MapDelete("/{customerName}/items/{productId:int}", (string customerName, int productId, ShoppingCartService shoppingCartService) =>
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                return Results.BadRequest("Customer name is required.");
            }

            var removed = shoppingCartService.RemoveItem(customerName, productId);
            return removed ? Results.NoContent() : Results.NotFound();
        });

        group.MapPost("/{customerName}/checkout", (string customerName, ShoppingCartService shoppingCartService) =>
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                return Results.BadRequest("Customer name is required.");
            }

            var cart = shoppingCartService.GetByCustomerName(customerName);
            if (cart is null)
            {
                return Results.NotFound();
            }

            if (cart.Items.Count == 0)
            {
                return Results.BadRequest("A cart must contain at least one item before checkout.");
            }

            var order = shoppingCartService.Checkout(customerName);
            return order is not null ? Results.Created($"/api/orders/{order.Id}", order) : Results.BadRequest();
        });

        return app;
    }
}
