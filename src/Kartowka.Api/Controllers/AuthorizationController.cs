using System.Net.Mime;
using Kartowka.Authorization.Core.Models;
using Kartowka.Authorization.Core.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Kartowka.Api.Controllers;

[ApiController, Route("api/authorization")]
public class AuthorizationController : ControllerBase
{
    [HttpPost("")]
    [SwaggerOperation("Authorize a user by login and password")]
    [SwaggerResponse(StatusCodes.Status200OK, "Access token.", typeof(TokenInfo), MediaTypeNames.Application.Json)]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Invalid credentials error.", 
        typeof(TokenInfo),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<TokenInfo>> AuthorizeAsync(
        [FromServices] IUserAuthorizationService userAuthorizationService,
        [FromBody] UserCredentials credentials
    )
    {
        var tokenInfo = await userAuthorizationService.AuthorizeAsync(credentials);
        return Ok(tokenInfo);
    }
}