using System.ComponentModel.DataAnnotations;
using Kartowka.Core.Resources;

namespace Kartowka.Packs.Core.Models;

public class UpdateAssetDetailsDto
{
    [StringLength(50,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.StringLength50)
    )]
    public string? DisplayName { get; set; }
}