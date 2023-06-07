using Kartowka.Authorization.Core.Contracts;
using Kartowka.Authorization.Core.Models;
using Kartowka.Authorization.Core.Resources;
using Kartowka.Authorization.Core.Services.Abstractions;
using Kartowka.Common.Crypto.Abstractions;
using Kartowka.Core;
using Kartowka.Core.Exceptions;
using Kartowka.Core.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Kartowka.Authorization.Core.Services;

public class UserAuthorizationService : IUserAuthorizationService
{
    private readonly CoreContext _context;

    private readonly IAccessTokenGenerator _accessTokenGenerator;

    private readonly IHasher _hasher;

    private readonly IStringLocalizer<ErrorMessages> _stringLocalizer;

    public UserAuthorizationService(
        CoreContext context,
        IAccessTokenGenerator accessTokenGenerator,
        IHasher hasher,
        IStringLocalizer<ErrorMessages> stringLocalizer
    )
    {
        _context = context;
        _accessTokenGenerator = accessTokenGenerator;
        _hasher = hasher;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<TokenInfo> AuthorizeAsync(UserCredentials credentials)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == credentials.EmailAddress);
        if (user == null)
        {
            throw new KartowkaNotFoundException(_stringLocalizer[nameof(ErrorMessages.InvalidUserCredentials)]);
        }

        if (credentials.Password is null || user.PasswordHash is null || user.PasswordSalt is null)
        {
            throw new KartowkaNotFoundException(_stringLocalizer[nameof(ErrorMessages.InvalidUserCredentials)]);
        }

        var passwordHashToCompare = _hasher.Hash(credentials.Password, user.PasswordSalt);
        if (!passwordHashToCompare.SequenceEqual(user.PasswordHash))
        {
            throw new KartowkaNotFoundException(_stringLocalizer[nameof(ErrorMessages.InvalidUserCredentials)]);
        }

        // This check must be run at the end.
        if (user.Status != UserStatus.Active)
        {
            throw new KartowkaException(_stringLocalizer[nameof(ErrorMessages.UserIsInactive)]);
        }

        return _accessTokenGenerator.GenerateToken(user);
    }
}