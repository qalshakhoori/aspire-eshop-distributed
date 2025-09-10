using Catalog.Data;
using MassTransit;
using ServiceDefaults.Messaging.Events;

namespace Catalog.Services;

public class ProductService(ProductDbContext dbContext, IBus bus)
{
    public async Task CreateProductAsync(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product productInDb, Product product)
    {
        ArgumentNullException.ThrowIfNull(productInDb);
        ArgumentNullException.ThrowIfNull(product);

        if (productInDb.Price != product.Price)
        {
            // publish event to message broker
            var IntegrationEvent = new ProductPriceChangedIntegrationEvent
            {
                ProductId = productInDb.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl
            };

            await bus.Publish(IntegrationEvent);
        }

        productInDb.Name = product.Name;
        productInDb.Description = product.Description;
        productInDb.Price = product.Price;
        productInDb.ImageUrl = product.ImageUrl;

        dbContext.Products.Update(productInDb);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Product productInDb)
    {
        ArgumentNullException.ThrowIfNull(productInDb);

        dbContext.Products.Remove(productInDb);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await dbContext.Products.FindAsync(id);
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        return await dbContext.Products.ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        return await dbContext
            .Products
            .AsNoTracking() // use as no tracking for read only operations, this makes ef core query faster
            .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
            .ToListAsync();
    }
}
