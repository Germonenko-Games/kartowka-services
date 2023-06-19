using System.Net.Mime;
using Kartowka.Api.Models;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;
using Kartowka.Packs.Core.Models.Enums;
using Kartowka.Packs.Core.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Kartowka.Api.Controllers;

[ApiController, Authorize, Route("api/packs")]
public class PacksController : ControllerBase
{
    private readonly IPacksService _packsService;

    public PacksController(IPacksService packsService)
    {
        _packsService = packsService;
    }

    [HttpGet("{packId:long}"), AllowAnonymous]
    [SwaggerOperation("Gets a pack by an ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Pack object", typeof(Pack), MediaTypeNames.Application.Json)]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Not found error",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<Pack>> GetPackAsync(
        [FromRoute] long packId,
        [FromQuery, SwaggerParameter("Specifies what related entities should be pulled as well.")]
        ICollection<PackProperties>? includeProperties
    )
    {
        var pack = await _packsService.GetPackAsync(packId, includeProperties);
        return Ok(pack);
    }

    [HttpPost("")]
    [SwaggerOperation("Creates a new pack.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Pack object", typeof(Pack), MediaTypeNames.Application.Json)]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Validation error",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<Pack>> CreatePackAsync([FromBody] CreatePackDto packDto)
    {
        var pack = await _packsService.CreatePackAsync(packDto);
        return Ok(pack);
    }

    [HttpPatch("{packId:long}")]
    [SwaggerOperation("Updates a pack with a given ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Updated pack", typeof(Pack), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(Pack), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not found error", typeof(Pack), MediaTypeNames.Application.Json)]
    public async Task<ActionResult<Pack>> UpdatePackAsync(
        [FromRoute] long packId,
        [FromBody, SwaggerRequestBody] UpdatePackDto packDto
    )
    {
        var pack = await _packsService.UpdatePackAsync(packId, packDto);
        return Ok(pack);
    }

    [HttpDelete("{packId:long}")]
    [SwaggerOperation("Removes a pack with a given ID.", "This endpoint is idempotent.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "No content success result")]
    public async Task<NoContentResult> RemovePackAsync([FromRoute] long packId)
    {
        await _packsService.RemovePackAsync(packId);
        return NoContent();
    }
}