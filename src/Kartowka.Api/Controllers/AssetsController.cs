using System.Net.Mime;
using Kartowka.Api.Models;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;
using Kartowka.Packs.Core.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
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

    [HttpPost(""), Consumes("multipart/form-data")]
    [SwaggerOperation("Upload asset")]
    [SwaggerResponse(StatusCodes.Status200OK, "New asset descriptor", typeof(Asset), "application/json")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ErrorResponse), "application/json")]
    public async Task<ActionResult<Asset>> UploadAssetsAsync(
        [FromForm] AssetUploadRequest uploadRequest,
        [FromServices] IStringLocalizer<Resources.ErrorMessages> errorMessages
    )
    {
        if (uploadRequest.Asset is null)
        {
            var message = errorMessages.GetString(nameof(Resources.ErrorMessages.AssetFileIsRequired));
            return BadRequest(new ErrorResponse(message));
        }

        var assetDto = new UploadAssetDto
        {
            Content = new MemoryStream(),
            DisplayName = uploadRequest.Asset.FileName,
            MimeType = uploadRequest.Asset.ContentType,
            PackId = uploadRequest.PackId,
        };

        await uploadRequest.Asset.CopyToAsync(assetDto.Content);
        assetDto.Content.Position = 0;

        var asset = await _assetsService.CreateAssetAsync(assetDto);

        return Ok(asset);
    }

    [HttpGet("{assetId:long}"), Consumes(MediaTypeNames.Application.Json)]
    [SwaggerOperation("Get asset metadata")]
    [SwaggerResponse(StatusCodes.Status200OK, "Updated asset", typeof(Asset), "application/json")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not found error", typeof(ErrorResponse), "application/json")]
    public async Task<ActionResult<Asset>> GetAssetAsync(long assetId)
    {
        var asset = await _assetsService.GetAssetAsync(assetId);
        return Ok(asset);
    }

    [HttpPatch("{assetId:long}"), Consumes(MediaTypeNames.Application.Json)]
    [SwaggerOperation("Update asset")]
    [SwaggerResponse(StatusCodes.Status200OK, "Updated asset", typeof(Asset), "application/json")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ErrorResponse), "application/json")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not found error", typeof(ErrorResponse), "application/json")]
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