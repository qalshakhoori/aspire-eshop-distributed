using System;

namespace Catalog.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products");

        // GET all
        group.MapGet("/", async (ProductService productService) =>
        {
            var products = await productService.GetProductsAsync();
            return Results.Ok(products);
        })
        .WithName("GetAllProducts")
        .Produces<IEnumerable<Product>>(StatusCodes.Status200OK);

        // GET by id
        group.MapGet("/{id:int}", async (int id, ProductService productService) =>
        {
            var product = await productService.GetProductByIdAsync(id);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        })
        .WithName("GetProductById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // Post (create a product)
        group.MapPost("/", async (Product product, ProductService productService) =>
        {
            await productService.CreateProductAsync(product);
            return Results.CreatedAtRoute("GetProductById", new { id = product.Id }, product);
        })
        .WithName("CreateProduct")
        .Produces<Product>(StatusCodes.Status201Created);

        // PUT (update a product)
        group.MapPut("/{id}", async (int id, Product inputProduct, ProductService productService) =>
        {
            var productInDb = await productService.GetProductByIdAsync(id);
            if (productInDb is null)
            {
                return Results.NotFound();
            }
            await productService.UpdateProductAsync(productInDb, inputProduct);
            return Results.NoContent();
        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // DELETE (delete a product)
        group.MapDelete("/{id}", async (int id, ProductService productService) =>
        {
            var productInDb = await productService.GetProductByIdAsync(id);
            if (productInDb is null)
            {
                return Results.NotFound();
            }
            await productService.DeleteProductAsync(productInDb);
            return Results.NoContent();
        })
        .WithName("DeleteProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
