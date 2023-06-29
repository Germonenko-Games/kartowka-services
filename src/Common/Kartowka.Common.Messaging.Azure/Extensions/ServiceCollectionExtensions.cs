using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kartowka.Common.Messaging.Azure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void RegisterAzureQueueReceiver<TModel>(
        this IServiceCollection services,
        Action<AzureQueueReceiverOptions<TModel>> configure
    )
    {
        services.AddSingleton(_ =>
        {
            var options = new AzureQueueReceiverOptions<TModel>();
            configure(options);
            return options;
        });
        services.TryAddSingleton<IReceiver<TModel>, AzureQueueReceiver<TModel>>();
    }

    public static void RegisterAzureQueuePublisher<TModel>(
        this IServiceCollection services,
        Action<AzureQueuePublisherOptions<TModel>> configure
    )
    {
        services.AddSingleton(_ =>
        {
            var options = new AzureQueuePublisherOptions<TModel>();
            configure(options);
            return options;
        });
        services.TryAddSingleton<IPublisher<TModel>, AzureQueuePublisher<TModel>>();
    }
}
