using System.ComponentModel.DataAnnotations;
using Kartowka.Common.Validation;
using Kartowka.Core;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Options;
using Kartowka.Packs.Core.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Kartowka.Packs.Core.Validators;

public class UserPacksLimitValidator : IAsyncValidator<Pack>
{
    private readonly CoreContext _context;

    private readonly IOptionsSnapshot<PacksOptions> _options;

    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    private int MaxPacksNumberPerUser => _options.Value.MaxPacksNumberPerUser;

    public UserPacksLimitValidator(
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
        var packsNumber = await _context.Packs.CountAsync(p => p.Id != pack.Id && p.AuthorId == pack.AuthorId);
        if (packsNumber < MaxPacksNumberPerUser)
        {
            return true;
        }

        var errorMessage = _stringLocalizer.GetString(
            nameof(PacksErrorMessages.PacksLimitExceeded),
            MaxPacksNumberPerUser
        );

        validationResults.Add(new (errorMessage, new []{nameof(Pack.AuthorId)}));
        return false;
    }
}