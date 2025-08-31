using System;
using Catalog.Models;

namespace Basket.APiClients;

public class CatalogAPIClient(HttpClient httpClient)
{
    public async Task<Product> GetProductById(int productId)
    {
        var response = await httpClient.GetAsync($"/products/{productId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>() ?? throw new Exception("Failed to deserialize product.");
    }
}
