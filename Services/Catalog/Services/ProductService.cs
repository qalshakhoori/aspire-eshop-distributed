using Catalog.Data;

namespace Catalog.Services;

public class ProductService(ProductDbContext dbContext)
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
}
