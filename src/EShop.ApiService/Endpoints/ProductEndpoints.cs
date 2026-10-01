using EShop.ApiService.Models;
using EShop.ApiService.Services;

namespace EShop.ApiService.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", (ProductService productService) =>
            Results.Ok(productService.GetAll()));

        group.MapGet("/{id:int}", (int id, ProductService productService) =>
        {
            var product = productService.GetById(id);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        });

        group.MapPost("/", (Product product, ProductService productService) =>
        {
            var created = productService.Create(product);
            return Results.Created($"/api/products/{created.Id}", created);
        });

        group.MapPut("/{id:int}", (int id, Product product, ProductService productService) =>
        {
            var updated = productService.Update(id, product);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", (int id, ProductService productService) =>
        {
            var deleted = productService.Delete(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
