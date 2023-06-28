using System.ComponentModel.DataAnnotations;
using Kartowka.Common.Validation;
using Kartowka.Core.Models;
using Kartowka.Core.Models.Enums;
using Kartowka.Packs.Core.Resources;
using Microsoft.Extensions.Localization;

namespace Kartowka.Packs.Core.Validators;

public class MusicQuestionContentValidator : IValidator<Question>
{
    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    public MusicQuestionContentValidator(IStringLocalizer<PacksErrorMessages> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    public bool Validate(Question question, ICollection<ValidationResult> validationResults)
    {
        if (question.ContentType != QuestionContentType.Music)
        {
            return true;
        }

        if (question.Asset is not null && question.Asset.AssetType == AssetType.Music)
        {
            return true;
        }

        var message = _stringLocalizer[nameof(PacksErrorMessages.MusicQuestionContentIsInvalid)];
        var error = new ValidationResult(message, new[]
        {
            nameof(Question.Asset),
            nameof(Question.ContentType)
        });
        validationResults.Add(error);

        return false;
    }
}