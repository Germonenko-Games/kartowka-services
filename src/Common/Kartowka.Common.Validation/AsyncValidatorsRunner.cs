using System.ComponentModel.DataAnnotations;
using Kartowka.Core.Exceptions;

namespace Kartowka.Common.Validation;

public class AsyncValidatorsRunner<TModel> : IAsyncValidatorsRunner<TModel>
{
    private readonly IEnumerable<IAsyncValidator<TModel>> _validators;

    public AsyncValidatorsRunner(IEnumerable<IAsyncValidator<TModel>> validators)
    {
        _validators = validators;
    }

    public async Task EnsureValidAsync(TModel model, string errorMessage)
    {
        var valid = true;
        var validationResults = new List<ValidationResult>(10);
        foreach (var validator in _validators)
        {
            var validationPassed = await validator.ValidateAsync(model, validationResults);
            if (!validationPassed)
            {
                valid = false;
            }
        }

        if (!valid)
        {
            throw new KartowkaValidationException(errorMessage)
            {
                Errors = validationResults,
            };
        }
    }
}