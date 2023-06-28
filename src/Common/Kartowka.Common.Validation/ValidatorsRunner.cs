using System.ComponentModel.DataAnnotations;
using Kartowka.Core.Exceptions;

namespace Kartowka.Common.Validation;

public class ValidatorsRunner<TModel> : IValidatorsRunner<TModel>
{
    private readonly IEnumerable<IValidator<TModel>> _validators;

    private readonly IEnumerable<IAsyncValidator<TModel>> _asyncValidators;

    public ValidatorsRunner(
        IEnumerable<IValidator<TModel>> validators,
        IEnumerable<IAsyncValidator<TModel>> asyncValidators
    )
    {
        _validators = validators;
        _asyncValidators = asyncValidators;
    }

    public async Task EnsureValidAsync(TModel model, string errorMessage)
    {
        var valid = true;
        var validationResults = new List<ValidationResult>(10);

        foreach (var validator in _validators)
        {
            var validationPassed = validator.Validate(model, validationResults);
            if (!validationPassed)
            {
                valid = false;
            }
        }

        foreach (var validator in _asyncValidators)
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