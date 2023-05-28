using Kartowka.Authorization.Core.Models;
using Kartowka.Core.Models;

namespace Kartowka.Authorization.Core.Contracts;

public interface IAccessTokenGenerator
{
    public TokenInfo GenerateToken(User user);
}
