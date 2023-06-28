using System.ComponentModel.DataAnnotations;
using Kartowka.Common.Validation;
using Kartowka.Core.Models;
using Kartowka.Core.Models.Enums;
using Kartowka.Packs.Core.Resources;
using Microsoft.Extensions.Localization;

namespace Kartowka.Packs.Core.Validators;

public class TextQuestionContentValidator : IValidator<Question>
{
    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    public TextQuestionContentValidator(IStringLocalizer<PacksErrorMessages> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    public bool Validate(Question question, ICollection<ValidationResult> validationResults)
    {
        if (question.ContentType != QuestionContentType.Text)
        {
            return true;
        }

        if (question.QuestionText is not null)
        {
            return true;
        }

        var message = _stringLocalizer[nameof(PacksErrorMessages.QuestionTextIsRequired)];
        var error = new ValidationResult(message, new[]
        {
            nameof(question.QuestionText),
            nameof(question.ContentType)
        });
        validationResults.Add(error);
        return false;
    }
}
