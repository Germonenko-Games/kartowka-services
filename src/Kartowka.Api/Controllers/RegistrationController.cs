using System.Net.Mime;
using Kartowka.Api.Models;
using Kartowka.Registration.Core.Models;
using Kartowka.Registration.Core.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Kartowka.Api.Controllers;

[ApiController, Route("api/registration")]
public class RegistrationController : ControllerBase
{
    [HttpPost("")]
    [SwaggerOperation("Registers a new user.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Success response")]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Validation error",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    public async Task<NoContentResult> RegisterUserAsync(
        [FromBody, SwaggerRequestBody()] UserData user,
        [FromServices] IUserRegistrationService registrationService
    )
    {
        await registrationService.RegisterUserAsync(user);
        return NoContent();
    }
}
