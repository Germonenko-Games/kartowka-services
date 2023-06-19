using Kartowka.Common.Localization.Extensions;
using Kartowka.Common.Validation;
using Kartowka.Core;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Options;
using Kartowka.Packs.Core.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Kartowka.Packs.Core.Validators;

public class PackQuestionsCategoriesLimitValidator : IAsyncValidator<Pack>
{
    private readonly CoreContext _context;

    private readonly IOptionsSnapshot<PacksOptions> _options;

    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    private int MaxQuestionsCategoriesPerPack => _options.Value.MaxQuestionsCategoriesPerPack;

    public PackQuestionsCategoriesLimitValidator(
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
        // Consider Pack.QuestionsCategories is unchanged
        if (pack.QuestionsCategories is null)
        {
            return true;
        }

        var questionsCategoriesCount = await _context.Packs
            .Where(p => p.Id == pack.Id)
            .Select(p => p.QuestionsCategories)
            .CountAsync();

        if (questionsCategoriesCount <= MaxQuestionsCategoriesPerPack)
        {
            return true;
        }

        var message = _stringLocalizer
            .GetString(nameof(PacksErrorMessages.PackQuestionsLimitExceeded))
            .Format(MaxQuestionsCategoriesPerPack);

        validationResults.Add(new(message, new[] { nameof(Pack.Questions) }));
        return false;
    }
}
