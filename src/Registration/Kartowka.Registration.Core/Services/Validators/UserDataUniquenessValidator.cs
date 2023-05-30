using Kartowka.Common.Validation;
using Kartowka.Core;
using Kartowka.Core.Models;
using Kartowka.Registration.Core.Models;
using Kartowka.Registration.Core.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Kartowka.Registration.Core.Services.Validators;

public class UserDataUniquenessValidator : IAsyncValidator<UserData>
{
    private readonly CoreContext _context;

    private readonly IStringLocalizer<ErrorMessages> _stringLocalizer;

    public UserDataUniquenessValidator(CoreContext context, IStringLocalizer<ErrorMessages> stringLocalizer)
    {
        _context = context;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<bool> ValidateAsync(
        UserData userData, 
        ICollection<ValidationResult> validationResults
    )
    {
        var duplicateUsers = await _context.Users
            .Where(u => u.EmailAddress == userData.EmailAddress || u.Username == userData.Username)
            .Select(u => new {u.Username, u.EmailAddress})
            .ToListAsync();

        if (duplicateUsers.Count == 0)
        {
            return true;
        }

        if (duplicateUsers.Any(u => u.EmailAddress == userData.EmailAddress))
        {
            var error = new ValidationResult(
                _stringLocalizer[nameof(ErrorMessages.EmailAddressIsInUse)],
                new[] { nameof(User.EmailAddress) }
            );
            validationResults.Add(error);
        }

        if (duplicateUsers.Any(u => u.Username == userData.Username))
        {
            var error = new ValidationResult(
                _stringLocalizer[nameof(ErrorMessages.UsernameIsInUse)],
                new[] { nameof(User.Username) }
            );
            validationResults.Add(error);
        }

        return false;
    }
}
