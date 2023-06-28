using System.ComponentModel.DataAnnotations;
using Kartowka.Core.Resources;

namespace Kartowka.Packs.Core.Models;

public class CreatePackDto
{
    public int AuthorId { get; set; }

    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.Required)
    )]
    [StringLength(50,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.StringLength50)
    )]
    public string Name { get; set; } = string.Empty;

    [Required(
        AllowEmptyStrings = true,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.Required)
    )]
    public string Description { get; set; } = string.Empty;
}