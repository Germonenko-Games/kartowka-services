using System.ComponentModel.DataAnnotations;
using Kartowka.Common.Validation;
using Kartowka.Core.Models;
using Kartowka.Core.Models.Enums;
using Kartowka.Packs.Core.Resources;
using Microsoft.Extensions.Localization;

namespace Kartowka.Packs.Core.Validators;

public class QuestionContentTypeValidator : IAsyncValidator<Question>
{
    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    private string QuestionTypeIsUnsupportedMessage => _stringLocalizer[nameof(PacksErrorMessages.QuestionTypeIsUnsupported)];

    public QuestionContentTypeValidator(IStringLocalizer<PacksErrorMessages> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    public Task<bool> ValidateAsync(Question question, ICollection<ValidationResult> validationResults)
    {
        switch (question.ContentType)
        {
            case QuestionContentType.Text:
                return Task.FromResult(true);
            default:
                validationResults.Add(
                    new ValidationResult(
                        QuestionTypeIsUnsupportedMessage,
                        new [] {nameof(Question.ContentType)}
                    )
                );
                return Task.FromResult(false);
        }
    }
}
