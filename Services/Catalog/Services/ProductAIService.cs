using System;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.VisualBasic;

namespace Catalog.Services;

public class ProductAIService(ProductDbContext dbContext,
    IChatClient chatClient,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    InMemoryCollection<int, ProductVector> productVectorCollection)
{
    public async Task<string> SupportAsync(string question)
    {
        var systemPrompt = """
        You are a useful assistant. 
        You always reply with a short and funny message. 
        If you do not know an answer, you say 'I don't know that.' 
        You only answer questions related to outdoor camping products. 
        For any other type of questions, explain to the user that you only answer outdoor camping products questions.
        At the end, Offer one of our products: Hiking Poles-$24, Outdoor Rain Jacket-$12, Outdoor Backpack-$32, Camping Tent-$22
        Do not store memory of the chat conversation.
        """;

        var chatHistory = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, question)
        };

        var resultPrompt = await chatClient.GetResponseAsync(chatHistory);
        return resultPrompt.Messages[0].Contents.ToString();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        // Embeddings should be generated ahead of time and not at the time of search
        // Usually there will be a seperate process that generated the embeddings on a schedual job, example: Hourl, Daily
        if (!await productVectorCollection.CollectionExistsAsync())
            await InitEmbeddingsAsync();

        var queryEmbeddings = await embeddingGenerator.GenerateVectorAsync(query);

        var vectorSearchOptions = new VectorSearchOptions<ProductVector>
        {
            Skip = 0,
            VectorProperty = m => m.Vector,
        };

        var result = productVectorCollection.SearchAsync(searchValue: queryEmbeddings, top: 5, options: vectorSearchOptions);

        var products = new List<Product>();
        await foreach (var productVector in result)
        {
            products.Add(new Product
            {
                Id = productVector.Record.Id,
                Name = productVector.Record.Name,
                Description = productVector.Record.Description,
                Price = productVector.Record.Price,
                ImageUrl = productVector.Record.ImageUrl
            });
        }

        return products;
    }

    private async Task InitEmbeddingsAsync()
    {
        await productVectorCollection.EnsureCollectionExistsAsync();

        var products = await dbContext.Products.ToListAsync();

        foreach (var product in products)
        {
            var productInfo = $"[{product.Name}] is a product that costs [{product.Price}] and is described as: [{product.Description}]";

            var productVector = new ProductVector
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Vector = await embeddingGenerator.GenerateVectorAsync(productInfo)
            };

            await productVectorCollection.UpsertAsync(productVector);
        }
    }
}
