using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Basket.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Services;

public class BasketService(IDistributedCache cache, CatalogAPIClient catalogAPIClient)
{
    public async Task<ShoppingCart> GetBasket(string userName)
    {
        var basket = await cache.GetStringAsync(userName);
        if (basket == null)
        {
            return null;
        }
        return JsonSerializer.Deserialize<ShoppingCart>(basket);
    }

    public async Task UpdateBasket(ShoppingCart basket)
    {
        // Before saving the basket, you might want to add some business logic here
        // e.g., validating items, checking stock, applying discounts, etc.

        foreach (var item in basket.Items)
        {
            var product = await catalogAPIClient.GetProductById(item.ProductId) ?? throw new Exception($"Product with ID {item.ProductId} not found.");
            item.Price = product.Price; // Update the price from the catalog
            item.ProductName = product.Name; // Update the product name from the catalog
        }

        await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));
    }

    public async Task DeleteBasket(string userName)
    {
        await cache.RemoveAsync(userName);
    }

    internal async Task UpdateProductPriceInBasketsAsync(int productId, decimal price)
    {
        var basket = await GetBasket("qalshakhoori");

        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            item.Price = price;
            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));
        }
    }
}
