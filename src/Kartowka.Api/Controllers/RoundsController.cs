using Kartowka.Api.Models;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;
using Kartowka.Packs.Core.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace Kartowka.Api.Controllers;

[ApiController, Authorize, Route("api/rounds"), Consumes(MediaTypeNames.Application.Json)]
public class RoundsController : ControllerBase
{
    private readonly IRoundsService _roundsService;

    public RoundsController(IRoundsService roundsService)
    {
        _roundsService = roundsService;
    }

    [HttpPost("")]
    [SwaggerOperation("Create a new round")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "New round object",
        typeof(Round),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Round validation error",
        typeof(Round),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<Round>> CreateRoundAsync(
        [FromBody, SwaggerRequestBody] CreateRoundDto roundDto
    )
    {
        var round = await _roundsService.CreateRoundAsync(roundDto);
        return Ok(round);
    }

    [HttpPatch("{roundId:long}")]
    [SwaggerOperation("Update a round")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Updated round object",
        typeof(Round),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Round validation error",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Round not found error",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<Round>> UpdateRoundAsync(
        [FromRoute] long roundId,
        [FromBody, SwaggerRequestBody] UpdateRoundDto roundDto
    )
    {
        var round = await _roundsService.UpdateRoundAsync(roundId, roundDto);
        return Ok(round);
    }

    [HttpDelete("{roundId:long}")]
    [SwaggerOperation("Remove a round", "This endpoint is idempotent.")]
    public async Task<NoContentResult> RemoveRoundAsync([FromRoute] long roundId)
    {
        await _roundsService.RemoveRoundAsync(roundId);
        return NoContent();
    }
}