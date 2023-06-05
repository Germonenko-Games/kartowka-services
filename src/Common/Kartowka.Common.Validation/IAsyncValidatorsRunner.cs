namespace Kartowka.Common.Validation;

public interface IAsyncValidatorsRunner<TModel>
{
    public Task EnsureValidAsync(TModel model, string errorMessage);
}