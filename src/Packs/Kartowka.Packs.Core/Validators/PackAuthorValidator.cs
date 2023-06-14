using System.ComponentModel.DataAnnotations;
using Kartowka.Common.Validation;
using Kartowka.Core;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Kartowka.Packs.Core.Validators;

public class PackAuthorValidator : IAsyncValidator<Pack>
{
    private readonly CoreContext _context;

    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    public PackAuthorValidator(CoreContext context, IStringLocalizer<PacksErrorMessages> stringLocalizer)
    {
        _context = context;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<bool> ValidateAsync(Pack pack, ICollection<ValidationResult> validationResults)
    {
        var authorExists = await _context.Users.AnyAsync(u => u.Id == pack.AuthorId);
        if (authorExists)
        {
            return true;
        }

        var errorMessage = _stringLocalizer[nameof(PacksErrorMessages.UserNotFound)];
        validationResults.Add(new (errorMessage, new []{nameof(Pack.AuthorId)}));

        return false;
    }
}