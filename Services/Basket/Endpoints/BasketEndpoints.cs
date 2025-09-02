using System;

namespace Basket.Endpoints;

public static class BasketEndpoints
{
    public static void MapBasketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("basket");

        group.MapGet("/{username}", async (string username, BasketService basketService) =>
        {
            var basket = await basketService.GetBasket(username);
            return basket is null ? Results.NotFound() : Results.Ok(basket);
        })
        .WithName("GetBasket")
        .Produces<ShoppingCart>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization()
        .WithOpenApi();

        group.MapPost("/", async (ShoppingCart basket, BasketService basketService) =>
        {
            await basketService.UpdateBasket(basket);
            return Results.Created("GetBasket", basket);
        })
        .WithName("UpdateBasket")
        .Produces<ShoppingCart>(StatusCodes.Status200OK)
        .RequireAuthorization()
        .WithOpenApi();

        group.MapDelete("/{username}", async (string username, BasketService basketService) =>
        {
            await basketService.DeleteBasket(username);
            return Results.NoContent();
        })
        .WithName("DeleteBasket")
        .Produces(StatusCodes.Status204NoContent)
        .RequireAuthorization()
        .WithOpenApi();
    }
}
