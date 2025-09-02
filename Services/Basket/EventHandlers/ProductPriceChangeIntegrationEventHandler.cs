using System;
using MassTransit;
using ServiceDefaults.Messaging.Events;

namespace Basket.EventHandlers;

public class ProductPriceChangeIntegrationEventHandler(BasketService basketService) : IConsumer<ProductPriceChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
    {
        var message = context.Message;

        ArgumentNullException.ThrowIfNull(message);

        await basketService.UpdateProductPriceInBasketsAsync(message.ProductId, message.Price);
    }
}
