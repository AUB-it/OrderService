using OrderService.Models;

namespace OrderService.Services;

public class ProductCatalogService
{
    private readonly HttpClient _httpClient;

    public ProductCatalogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Orderitem?> GetProductByIdAsync(Guid productId)
    {
        var response = await _httpClient.GetAsync($"catalog/{productId}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<Orderitem>();
    }
}