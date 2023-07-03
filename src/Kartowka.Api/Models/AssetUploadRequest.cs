using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kartowka.Api.Models;

public class AssetUploadRequest
{
    [FromForm(Name = "asset")]
    [Required(
        ErrorMessageResourceType = typeof(Resources.ErrorMessages),
        ErrorMessageResourceName = nameof(Resources.ErrorMessages.AssetFileIsRequired)
    )]
    public IFormFile? Asset { get; set; }

    [FromForm(Name = "packId")]
    [Required(
        ErrorMessageResourceType = typeof(Resources.ErrorMessages),
        ErrorMessageResourceName = nameof(Resources.ErrorMessages.PackIdIsRequired)
    )]
    public long PackId { get; set; }
}
