using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceDefaults.Messaging;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            config.SetInMemorySagaRepositoryProvider();

            config.AddConsumers(assemblies);
            config.AddSagaStateMachines(assemblies);
            config.AddSagas(assemblies);
            config.AddActivities(assemblies);

            config.UsingRabbitMq((ctx, config) =>
            {
                var configuration = ctx.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("rabbitmq");

                config.Host(connectionString);
                config.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
