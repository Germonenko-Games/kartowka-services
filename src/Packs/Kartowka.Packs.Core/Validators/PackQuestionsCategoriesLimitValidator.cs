using Kartowka.Common.Validation;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Options;
using Kartowka.Packs.Core.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Kartowka.Packs.Core.Validators;

public class PackQuestionsCategoriesLimitValidator : IValidator<Pack>
{
    private readonly IOptionsSnapshot<PacksOptions> _options;

    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    private int MaxQuestionsCategoriesPerPack => _options.Value.MaxQuestionsCategoriesPerPack;

    public PackQuestionsCategoriesLimitValidator(
        IOptionsSnapshot<PacksOptions> options,
        IStringLocalizer<PacksErrorMessages> stringLocalizer
    )
    {
        _options = options;
        _stringLocalizer = stringLocalizer;
    }

    public bool Validate(Pack pack, ICollection<ValidationResult> validationResults)
    {
        // Consider Pack.QuestionsCategories is unchanged
        if (pack.QuestionsCategories is null)
        {
            return true;
        }

        if (pack.QuestionsCategories.Count <= MaxQuestionsCategoriesPerPack)
        {
            return true;
        }

        var message = _stringLocalizer.GetString(
            nameof(PacksErrorMessages.PackQuestionsLimitExceeded),
            MaxQuestionsCategoriesPerPack
        );

        validationResults.Add(new(message, new[] { nameof(Pack.Questions) }));
        return false;
    }
}
