using System.Reflection;
using Basket.Endpoints;
using Microsoft.Extensions.Options;
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

builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer(
        serviceName: "keycloak",
        realm: "eshop",
        configureOptions: options =>
        {
            options.RequireHttpsMetadata = false;
            options.Audience = "account";
        });
builder.Services.AddAuthorization();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();
app.MapBasketEndpoints();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.Run();
