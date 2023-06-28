using System.ComponentModel.DataAnnotations;
using Kartowka.Common.Validation;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Options;
using Kartowka.Packs.Core.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Kartowka.Packs.Core.Validators;

public class PackQuestionsLimitValidator : IValidator<Pack>
{
    private readonly IOptionsSnapshot<PacksOptions> _options;

    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    private int MaxQuestionsNumberPerPack => _options.Value.MaxQuestionsNumberPerPack;

    public PackQuestionsLimitValidator(
        IOptionsSnapshot<PacksOptions> options,
        IStringLocalizer<PacksErrorMessages> stringLocalizer
    )
    {
        _options = options;
        _stringLocalizer = stringLocalizer;
    }

    public bool Validate(Pack pack, ICollection<ValidationResult> validationResults)
    {
        // Consider Pack.Questions is unchanged
        if (pack.Questions is null)
        {
            return true;
        }

        if (pack.Questions.Count <= MaxQuestionsNumberPerPack)
        {
            return true;
        }

        var message = _stringLocalizer.GetString(
            nameof(PacksErrorMessages.PackQuestionsLimitExceeded),
            MaxQuestionsNumberPerPack
        );

        validationResults.Add(new (message, new []{nameof(Pack.Questions)}));
        return false;
    }
}