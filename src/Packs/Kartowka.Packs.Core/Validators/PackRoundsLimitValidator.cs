using System.ComponentModel.DataAnnotations;
using Kartowka.Common.Validation;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Options;
using Kartowka.Packs.Core.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Kartowka.Packs.Core.Validators;

public class PackRoundsLimitValidator : IValidator<Pack>
{
    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    private readonly IOptionsSnapshot<PacksOptions> _options;

    private int MaxRoundsNumberPerPack => _options.Value.MaxRoundsNumberPerPack;

    public PackRoundsLimitValidator(
        IStringLocalizer<PacksErrorMessages> stringLocalizer,
        IOptionsSnapshot<PacksOptions> options
    )
    {
        _stringLocalizer = stringLocalizer;
        _options = options;
    }

    public bool Validate(Pack pack, ICollection<ValidationResult> validationResults)
    {
        // Consider Pack.Rounds is unchanged
        if (pack.Rounds is null)
        {
            return true;
        }

        if (pack.Rounds.Count <= MaxRoundsNumberPerPack)
        {
            return true;
        }

        var message = _stringLocalizer.GetString(
            nameof(PacksErrorMessages.PackQuestionsLimitExceeded),
            MaxRoundsNumberPerPack
        );

        validationResults.Add(new (message, new []{nameof(Pack.Questions)}));
        return false;
    }
}