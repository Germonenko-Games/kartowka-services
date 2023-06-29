using Azure.Storage.Queues;
using System.Text.Json;

namespace Kartowka.Common.Messaging.Azure;

public class AzureQueuePublisher<TModel> : IPublisher<TModel>
{
    private readonly AzureQueuePublisherOptions<TModel> _options;

    private readonly QueueServiceClient _queueServiceClient;

    private readonly QueueClient _queueClient;

    public AzureQueuePublisher(
        AzureQueuePublisherOptions<TModel> options,
        QueueServiceClient queueServiceClient
    )
    {
        _options = options;
        _queueServiceClient = queueServiceClient;
        _queueClient = _queueServiceClient.GetQueueClient(_options.QueueName);
    }

    public async Task PublishAsync(TModel model)
    {
        var message = JsonSerializer.Serialize(model, _options.SerializerOptions);
        await _queueClient.SendMessageAsync(message);
    }
}
