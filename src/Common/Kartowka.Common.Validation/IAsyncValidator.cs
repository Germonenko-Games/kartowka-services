using System.ComponentModel.DataAnnotations;

namespace Kartowka.Common.Validation;

public interface IAsyncValidator<TModel>
{
    public Task<bool> ValidateAsync(TModel model, ICollection<ValidationResult> validationResults);
}