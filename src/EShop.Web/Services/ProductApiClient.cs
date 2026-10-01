using EShop.Web.Models;

namespace EShop.Web.Services;

public sealed class ProductApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await httpClient.GetFromJsonAsync<List<ProductDto>>(
            "/api/products",
            cancellationToken);

        return products ?? [];
    }
}
