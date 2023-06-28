namespace Kartowka.Common.Validation;

public interface IValidatorsRunner<TModel>
{
    public Task EnsureValidAsync(TModel model, string errorMessage);
}