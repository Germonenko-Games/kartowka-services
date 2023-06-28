using System.ComponentModel.DataAnnotations;
using Kartowka.Core.Resources;

namespace Kartowka.Packs.Core.Models;

public class UploadAssetDto
{
    [StringLength(400,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.StringLength400)
    )]
    public string? DisplayName { get; set; }

    public required Stream Content { get; set; }

    public required string MimeType { get; set; }

    public long PackId { get; set; }
}