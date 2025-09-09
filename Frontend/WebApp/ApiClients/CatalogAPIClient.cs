using System;
using Catalog.Models;

namespace WebApp.ApiClients;

public class CatalogAPIClient(HttpClient client)
{
    public async Task<List<Product>> GetProductsAsync()
    {
        var response = await client.GetAsync("/products");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Product>>() ?? [];
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        var response = await client.GetAsync($"/products/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>() ?? throw new Exception("Product not found");
    }

    public async Task<string> SupportProducts(string query)
    {
        var response = await client.GetFromJsonAsync<string>($"/products/support/{query}");
        return response!;
    }
}
