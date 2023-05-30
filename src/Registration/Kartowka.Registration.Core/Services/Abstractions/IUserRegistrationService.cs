using Kartowka.Core.Models;
using Kartowka.Registration.Core.Models;

namespace Kartowka.Registration.Core.Services.Abstractions;

public interface IUserRegistrationService
{
    public Task<User> RegisterUserAsync(UserData userData);
}
