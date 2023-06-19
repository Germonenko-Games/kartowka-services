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

public class PackQuestionsLimitValidator : IAsyncValidator<Pack>
{
    private readonly CoreContext _context;

    private readonly IOptionsSnapshot<PacksOptions> _options;

    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    private int MaxQuestionsNumberPerPack => _options.Value.MaxQuestionsNumberPerPack;

    public PackQuestionsLimitValidator(
        CoreContext context,
        IOptionsSnapshot<PacksOptions> options,
        IStringLocalizer<PacksErrorMessages> stringLocalizer
    )
    {
        _context = context;
        _options = options;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<bool> ValidateAsync(Pack pack, ICollection<ValidationResult> validationResults)
    {
        // Consider Pack.Questions is unchanged
        if (pack.Questions is null)
        {
            return true;
        }

        var questionsCount = await _context.Packs
            .Where(p => p.Id == pack.Id)
            .Select(p => p.Questions)
            .CountAsync();

        if (questionsCount <= MaxQuestionsNumberPerPack)
        {
            return true;
        }

        var message = _stringLocalizer
            .GetString(nameof(PacksErrorMessages.PackQuestionsLimitExceeded))
            .Format(MaxQuestionsNumberPerPack);

        validationResults.Add(new (message, new []{nameof(Pack.Questions)}));
        return false;
    }
}