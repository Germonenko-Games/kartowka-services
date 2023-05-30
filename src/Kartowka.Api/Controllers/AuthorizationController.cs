using Kartowka.Authorization.Core.Models;
using Kartowka.Authorization.Core.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Kartowka.Api.Controllers;

[ApiController, Route("api/authorization")]
public class AuthorizationController : ControllerBase
{
    [HttpPost("")]
    public async Task<ActionResult<TokenInfo>> AuthorizeAsync(
        [FromServices] IUserAuthorizationService userAuthorizationService,
        [FromBody] UserCredentials credentials
    )
    {
        var tokenInfo = await userAuthorizationService.AuthorizeAsync(credentials);
        return Ok(tokenInfo);
    }
}