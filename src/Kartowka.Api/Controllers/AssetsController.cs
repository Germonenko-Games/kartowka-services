using System.Net.Mime;
using Kartowka.Api.Models;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;
using Kartowka.Packs.Core.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Kartowka.Api.Controllers;

[ApiController, Authorize, Route("api/assets")]
public class AssetsController : ControllerBase
{
    private readonly IAssetsService _assetsService;

    public AssetsController(IAssetsService assetsService)
    {
        _assetsService = assetsService;
    }

    [HttpPatch("{assetId:long}")]
    [SwaggerOperation("Update asset")]
    [SwaggerResponse(StatusCodes.Status200OK, "Updated asset", typeof(Asset), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ErrorResponse), MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not found error", typeof(ErrorResponse), MediaTypeNames.Application.Json)]
    public async Task<ActionResult<Asset>> UpdateAssetAsync(
        long assetId,
        [FromBody] UpdateAssetDetailsDto assetDto
    )
    {
        var asset = await _assetsService.UpdateAssetDetailsAsync(assetId, assetDto);
        return Ok(asset);
    }

    [HttpDelete("{assetId:long}")]
    [SwaggerOperation("Remove asset", "This endpoint is idempotent.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Successful no content result.")]
    public async Task<NoContentResult> RemoveAssetAsync(long assetId)
    {
        await _assetsService.RemoveAssetAsync(assetId);
        return NoContent();
    }
}