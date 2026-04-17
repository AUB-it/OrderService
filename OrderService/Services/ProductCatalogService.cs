using OrderService.Models;

namespace OrderService.Services;

public class ProductCatalogService
{
    private readonly IHttpClientFactory _httpClient;

    public ProductCatalogService(IHttpClientFactory httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Orderitem?> GetProductByIdAsync(Guid productId)
    {
        var client = _httpClient.CreateClient("Catalog");
        
        var response = await client.GetAsync($"catalog/{productId}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<Orderitem>();
    }
}