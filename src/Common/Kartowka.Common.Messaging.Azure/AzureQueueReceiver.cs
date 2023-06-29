using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Kartowka.Common.Messaging.Models;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Kartowka.Common.Messaging.Azure;

public class AzureQueueReceiver<TModel> : IReceiver<TModel>
{
    private readonly AzureQueueReceiverOptions<TModel> _options;

    private readonly QueueServiceClient _queueServiceClient;

    private readonly QueueClient _queueClient;

    private static readonly TimeSpan MessageTimeout = TimeSpan.FromSeconds(30);

    private static readonly TimeSpan MessagePollInterval = TimeSpan.FromSeconds(5);

    public AzureQueueReceiver(
        AzureQueueReceiverOptions<TModel> options,
        QueueServiceClient queueServiceClient
    )
    {
        _options = options;
        _queueServiceClient = queueServiceClient;
        _queueClient = _queueServiceClient.GetQueueClient(options.QueueName);
    }

    public Task CompleteAsync(ConsumeContext<TModel> context)
    {
        return _queueClient.DeleteMessageAsync(context.MessageId, context.PopReceipt);
    }

    public async Task DeadLetterAsync(ConsumeContext<TModel> context)
    {
        if (_options.DeadLetterQueueName is not null)
        {
            await _queueClient.SendMessageAsync(context.Body?.ToString());
        }

        await _queueClient.DeleteMessageAsync(context.MessageId, context.PopReceipt);
    }

    public async Task<ConsumeContext<TModel>?> ReceiveNextAsync(CancellationToken cancellationToken)
    {
        QueueMessage? message = null;
        while (message?.Body is null && !cancellationToken.IsCancellationRequested)
        {
            message = await _queueClient.ReceiveMessageAsync(MessageTimeout, cancellationToken);
            if (message?.Body is not null)
            {
                break;
            }

            await Task.Delay(MessagePollInterval, cancellationToken);
        }

        if (message is null)
        {
            return null;
        }

        var model = JsonSerializer.Deserialize<TModel>(message.Body, _options.SerializerOptions);
        if (model == null)
        {
            throw new SerializationException(
                $"Unable to serialize '{message.Body.ToString()}' as {typeof(TModel).Name}"
            );
        }

        return new ConsumeContext<TModel>
        {
            Body = model,
            RawBody = message.Body.ToString(),
            MessageId = message.MessageId,
            PopReceipt = message.PopReceipt,
        };
    }
}
