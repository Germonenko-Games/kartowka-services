using System.ComponentModel.DataAnnotations;

namespace Kartowka.Common.Validation;

public interface IValidator<TModel>
{
    public bool Validate(TModel model, ICollection<ValidationResult> validationResults);
}