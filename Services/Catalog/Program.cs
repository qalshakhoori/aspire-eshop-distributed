using System.Reflection;
using Catalog;
using Catalog.Endpoints;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.SemanticKernel;
using ServiceDefaults.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProductDbContext>("catalog-db");
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductAIService>();
builder.Services.AddMassTransitWithRabbitMq(Assembly.GetExecutingAssembly());
// Register Ollama-based chat & embeddings
// builder.AddOllamaSharpChatClient("ollama-llama3-2");
builder.AddOllamaApiClient("ollama-llama3-2").AddChatClient();
builder.AddOllamaApiClient("ollama-all-minilm").AddEmbeddingGenerator();

// Register an in-memory vector store
// builder.Services.AddInMemoryVectorStoreRecordCollection<int, ProductVector>("products");

builder.Services.AddSingleton(sp =>
{
    var collection = new Microsoft.SemanticKernel.Connectors.InMemory.InMemoryCollection<int, ProductVector>("products");

    return collection;
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

// Apply migrations at startup
app.UseMigrations();
// Map product endpoints
app.MapProductEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();