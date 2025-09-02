using System.Reflection;
using Catalog;
using Catalog.Endpoints;
using ServiceDefaults.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProductDbContext>("catalog-db");
builder.Services.AddScoped<ProductService>();
builder.Services.AddMassTransitWithRabbitMq(Assembly.GetExecutingAssembly());

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