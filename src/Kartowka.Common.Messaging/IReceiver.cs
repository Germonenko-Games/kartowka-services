using Kartowka.Common.Messaging.Models;

namespace Kartowka.Common.Messaging;

public interface IReceiver<TMessage>
{
    public Task<ConsumeContext<TMessage>?> ReceiveNextAsync(CancellationToken cancellationToken);

    public Task CompleteAsync(ConsumeContext<TMessage> context);

    public Task DeadLetterAsync(ConsumeContext<TMessage> context);
}
