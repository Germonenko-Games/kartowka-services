using System.Net.Mime;
using Kartowka.Api.Models;
using Kartowka.Api.Options;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;
using Kartowka.Packs.Core.Models.Enums;
using Kartowka.Packs.Core.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
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
    [SwaggerOperation("Get pack by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Pack object", typeof(Pack), MediaTypeNames.Application.Json)]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Not found error",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<Pack>> GetPackAsync(
        [FromRoute] long packId,
        [FromQuery(Name = "properties"), SwaggerParameter("Specifies what related entities should be pulled as well.")]
        ICollection<PackProperties>? includeProperties
    )
    {
        var pack = await _packsService.GetPackAsync(packId, includeProperties);
        return Ok(pack);
    }

    [HttpPost("")]
    [SwaggerOperation("Create a new pack")]
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
    [SwaggerOperation("Update a pack")]
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
    [SwaggerOperation("Remove a pack", "This endpoint is idempotent.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "No content success result")]
    public async Task<NoContentResult> RemovePackAsync([FromRoute] long packId)
    {
        await _packsService.RemovePackAsync(packId);
        return NoContent();
    }

    [HttpPost("{packId:long}/assets/{fileName}")]
    [Consumes("image/jpeg", "image/png", "audio/mpeg", "audio/ogg")]
    [SwaggerOperation("Upload a new asset")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Uploaded asset descriptor",
        typeof(Asset),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Validation error",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Pack not found error",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<Asset>> UploadAssetAsync(
        [FromRoute] long packId,
        [FromRoute] string fileName,
        [FromServices] IAssetsService assetsService,
        [FromServices] IOptionsSnapshot<UploadLimitsOptions> uploadOptions,
        [FromServices] IStringLocalizer<Resources.ErrorMessages> errorMessages
    )
    {
        var fileSizeLimit = uploadOptions.Value.MaxAssetFileSizeMb * 1024 * 1024;
        var fileSizeExceedsLimit = Request.ContentLength > fileSizeLimit;

        if (fileSizeExceedsLimit)
        {
            var message = errorMessages.GetString(
                nameof(Resources.ErrorMessages.FileSizeLimitExceeded),
                fileSizeLimit
            );
            var response = new ErrorResponse(message);
            return BadRequest(response);
        }

        using var inMemoryContentStream = new MemoryStream();
        await Request.Body.CopyToAsync(inMemoryContentStream);
        inMemoryContentStream.Position = 0;

        var uploadDto = new UploadAssetDto
        {
            MimeType = Request.ContentType ?? string.Empty,
            DisplayName = fileName,
            Content = inMemoryContentStream,
            PackId = packId
        };

        var assetDescriptor = await assetsService.CreateAssetAsync(uploadDto);
        return Ok(assetDescriptor);
    }
}