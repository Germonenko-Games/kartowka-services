using Kartowka.Common.Messaging.Models;

namespace Kartowka.Common.Messaging;

public interface IConsumer<TModel>
{
    public Task ConsumeAsync(ConsumeContext<TModel> model, CancellationToken cancellationToken);
}
