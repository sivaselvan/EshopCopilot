using System.Net;
using EShop.Web.Models;

namespace EShop.Web.Services;

public sealed class CustomerApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await httpClient.GetFromJsonAsync<List<CustomerDto>>(
            "/api/customers",
            cancellationToken);

        return customers ?? [];
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"/api/customers/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await ReadContentAsync(response, cancellationToken);
    }

    public async Task<CustomerDto> CreateAsync(CustomerDto customer, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("/api/customers", customer, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await ReadContentAsync(response, cancellationToken);
    }

    public async Task UpdateAsync(CustomerDto customer, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(
            $"/api/customers/{customer.Id}",
            customer,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"/api/customers/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private static async Task<CustomerDto> ReadContentAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>(cancellationToken);
        return customer ?? throw new InvalidOperationException("The API response did not contain a customer.");
    }
}
