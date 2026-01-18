using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Lib.Shared.Messaging.Configuration;

public static class MassTransitExtensions
{
    /// <summary>
    /// Configura MassTransit com RabbitMQ para um microsserviço que apenas publica eventos
    /// </summary>
    public static IServiceCollection AddMessagingPublisher(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["Messaging:RabbitMQ:Host"] ?? "localhost";
                var username = configuration["Messaging:RabbitMQ:Username"] ?? "guest";
                var password = configuration["Messaging:RabbitMQ:Password"] ?? "guest";

                cfg.Host(host, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    /// <summary>
    /// Configura MassTransit com RabbitMQ para um microsserviço que consome um único tipo de evento
    /// </summary>
    public static IServiceCollection AddMessagingConsumer<TConsumer>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TConsumer : class, IConsumer
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["Messaging:RabbitMQ:Host"] ?? "localhost";
                var username = configuration["Messaging:RabbitMQ:Username"] ?? "guest";
                var password = configuration["Messaging:RabbitMQ:Password"] ?? "guest";

                cfg.Host(host, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.UseMessageRetry(r => r.Intervals(
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                ));

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    /// <summary>
    /// Configura MassTransit com RabbitMQ para um microsserviço que consome múltiplos tipos de eventos
    /// </summary>
    public static IServiceCollection AddMessagingConsumers(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IRegistrationConfigurator> configureConsumers,
        string serviceName)
    {
        services.AddMassTransit(x =>
        {
            configureConsumers(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["Messaging:RabbitMQ:Host"] ?? "localhost";
                var username = configuration["Messaging:RabbitMQ:Username"] ?? "guest";
                var password = configuration["Messaging:RabbitMQ:Password"] ?? "guest";

                cfg.Host(host, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.UseMessageRetry(r => r.Intervals(
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                ));

                // Criar endpoint com prefixo do serviço
                cfg.ConfigureEndpoints(context, new DefaultEndpointNameFormatter(serviceName, false));
            });
        });

        return services;
    }
}
