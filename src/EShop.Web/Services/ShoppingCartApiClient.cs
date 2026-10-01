using System.Net;
using EShop.Web.Models;

namespace EShop.Web.Services;

public sealed class ShoppingCartApiClient(HttpClient httpClient)
{
    public async Task<ShoppingCartDto?> GetAsync(string customerName, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"/api/carts/{Uri.EscapeDataString(customerName)}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await ReadContentAsync<ShoppingCartDto>(response, cancellationToken);
    }

    public async Task<ShoppingCartDto> AddItemAsync(
        string customerName,
        AddCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            $"/api/carts/{Uri.EscapeDataString(customerName)}/items",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        return await ReadContentAsync<ShoppingCartDto>(response, cancellationToken);
    }

    public async Task RemoveItemAsync(
        string customerName,
        int productId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync(
            $"/api/carts/{Uri.EscapeDataString(customerName)}/items/{productId}",
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task<OrderDto> CheckoutAsync(string customerName, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync(
            $"/api/carts/{Uri.EscapeDataString(customerName)}/checkout",
            content: null,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        return await ReadContentAsync<OrderDto>(response, cancellationToken);
    }

    private static async Task<T> ReadContentAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        return content ?? throw new InvalidOperationException("The API response did not contain the expected data.");
    }
}
