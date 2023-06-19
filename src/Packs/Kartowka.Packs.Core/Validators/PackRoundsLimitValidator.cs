using System.ComponentModel.DataAnnotations;
using Kartowka.Common.Localization.Extensions;
using Kartowka.Common.Validation;
using Kartowka.Core;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Options;
using Kartowka.Packs.Core.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Kartowka.Packs.Core.Validators;

public class PackRoundsLimitValidator : IAsyncValidator<Pack>
{
    private readonly CoreContext _context;

    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    private readonly IOptionsSnapshot<PacksOptions> _options;

    private int MaxRoundsNumberPerPack => _options.Value.MaxRoundsNumberPerPack;

    public PackRoundsLimitValidator(
        CoreContext context,
        IStringLocalizer<PacksErrorMessages> stringLocalizer,
        IOptionsSnapshot<PacksOptions> options
    )
    {
        _context = context;
        _stringLocalizer = stringLocalizer;
        _options = options;
    }

    public async Task<bool> ValidateAsync(Pack pack, ICollection<ValidationResult> validationResults)
    {
        // Consider Pack.Rounds is unchanged
        if (pack.Rounds is null)
        {
            return true;
        }

        var roundsCount = await _context.Packs
            .Where(p => p.Id == pack.Id)
            .Select(p => p.Rounds)
            .CountAsync();

        if (roundsCount <= MaxRoundsNumberPerPack)
        {
            return true;
        }

        var message = _stringLocalizer
            .GetString(nameof(PacksErrorMessages.PackQuestionsLimitExceeded))
            .Format(MaxRoundsNumberPerPack);

        validationResults.Add(new (message, new []{nameof(Pack.Questions)}));
        return false;
    }
}