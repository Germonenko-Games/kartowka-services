using Microsoft.Extensions.DependencyInjection;

namespace Kartowka.Common.Messaging;

public static class ServiceCollectionExtensions
{
    public static void ConfigureEndpoint<TModel>(
        this IServiceCollection services,
        Action<EndpointOptions<TModel>> configure
    )
    {
        services.AddSingleton((_) =>
        {
            var options = new EndpointOptions<TModel>();
            configure(options);
            return options;
        });
        services.AddHostedService<ConsumeEndpoint<TModel>>();
    }
}
