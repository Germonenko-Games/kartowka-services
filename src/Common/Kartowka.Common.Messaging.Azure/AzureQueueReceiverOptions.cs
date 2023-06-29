using System.Text.Json;

namespace Kartowka.Common.Messaging.Azure;

public class AzureQueueReceiverOptions<TModel>
{
    public string QueueName { get; set; } = string.Empty;

    public string? DeadLetterQueueName { get; set; }

    public JsonSerializerOptions SerializerOptions { get; set; } = GetDefaultSerializerOptions();

    public static JsonSerializerOptions GetDefaultSerializerOptions()
        => new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
}
