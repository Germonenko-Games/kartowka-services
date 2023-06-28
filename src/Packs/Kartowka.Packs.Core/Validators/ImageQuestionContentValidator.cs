using System.ComponentModel.DataAnnotations;
using Kartowka.Common.Validation;
using Kartowka.Core.Models;
using Kartowka.Core.Models.Enums;
using Kartowka.Packs.Core.Resources;
using Microsoft.Extensions.Localization;

namespace Kartowka.Packs.Core.Validators;

public class ImageQuestionContentValidator : IValidator<Question>
{
    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    public ImageQuestionContentValidator(IStringLocalizer<PacksErrorMessages> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    public bool Validate(Question question, ICollection<ValidationResult> validationResults)
    {
        if (question.ContentType != QuestionContentType.Image)
        {
            return true;
        }

        if (question.Asset is not null && question.Asset.AssetType == AssetType.Image)
        {
            return true;
        }

        var message = _stringLocalizer[nameof(PacksErrorMessages.ImageQuestionContentIsInvalid)];
        var error = new ValidationResult(message, new[]
        {
            nameof(Question.Asset),
            nameof(Question.ContentType)
        });
        validationResults.Add(error);

        return false;
    }
}