using Kartowka.Common.Crypto.Abstractions;
using Kartowka.Common.Validation;
using Kartowka.Core;
using Kartowka.Core.Constants;
using Kartowka.Core.Models;
using Kartowka.Core.Models.Enums;
using Kartowka.Registration.Core.Models;
using Kartowka.Registration.Core.Resources;
using Kartowka.Registration.Core.Services.Abstractions;
using Microsoft.Extensions.Localization;
using Microsoft.FeatureManagement;

namespace Kartowka.Registration.Core.Services;

public class UserRegistrationService : IUserRegistrationService
{
    private readonly IFeatureManager _featureManger;

    private readonly IStringLocalizer<ErrorMessages> _stringLocalizer;

    private readonly CoreContext _context;

    private readonly IHasher _hasher;

    private readonly IAsyncValidatorsRunner<UserData> _validatorsRunner;

    public UserRegistrationService(
        IFeatureManager featureManager,
        IStringLocalizer<ErrorMessages> stringLocalizer,
        CoreContext context,
        IHasher hasher,
        IAsyncValidatorsRunner<UserData> validatorsRunner
    )
    {
        _featureManger = featureManager;
        _stringLocalizer = stringLocalizer;
        _context = context;
        _hasher = hasher;
        _validatorsRunner = validatorsRunner;
    }

    public async Task<User> RegisterUserAsync(UserData userData)
    {
        await _validatorsRunner.EnsureValidAsync(
            userData,
            _stringLocalizer[nameof(ErrorMessages.UserDataIsInvalid)]
        );

        var activateUser = await _featureManger.IsEnabledAsync(FeatureFlags.AutoActivateUsers);

        var now = DateTimeOffset.UtcNow;
        var user = new User
        {
            EmailAddress = userData.EmailAddress,
            Username = userData.Username,
            CreatedDate = now,
            LastOnlineDate = now,
            Status = activateUser ? UserStatus.Active : UserStatus.Unverified,
        };

        (user.PasswordHash, user.PasswordSalt) = _hasher.Hash(userData.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
}