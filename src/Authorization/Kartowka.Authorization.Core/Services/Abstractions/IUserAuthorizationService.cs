using Kartowka.Authorization.Core.Models;

namespace Kartowka.Authorization.Core.Services.Abstractions;

public interface IUserAuthorizationService
{
    public Task<TokenInfo> AuthorizeAsync(UserCredentials credentials);
}
