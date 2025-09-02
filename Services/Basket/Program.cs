using System.Reflection;
using Basket.Endpoints;
using ServiceDefaults.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRedisDistributedCache(connectionName: "cache");

builder.Services.AddScoped<BasketService>();

builder.Services.AddHttpClient<CatalogAPIClient>(client =>
{
    client.BaseAddress = new("https+http://catalog-srv");
});

builder.Services.AddMassTransitWithRabbitMq(Assembly.GetExecutingAssembly());

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapBasketEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
