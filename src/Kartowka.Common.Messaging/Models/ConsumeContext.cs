namespace Kartowka.Common.Messaging.Models;

public class ConsumeContext<TModel>
{
    public required string PopReceipt { get; set; }

    public required string MessageId { get; set; }

    public required TModel Body { get; set; }

    public required string RawBody { get; set; }
}
