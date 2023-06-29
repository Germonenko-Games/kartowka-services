namespace Kartowka.Common.Messaging;

public interface IPublisher<TModel>
{
    public Task PublishAsync(TModel model);
}
