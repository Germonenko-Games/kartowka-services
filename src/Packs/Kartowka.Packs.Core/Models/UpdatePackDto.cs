using System.ComponentModel.DataAnnotations;
using Kartowka.Core.Resources;

namespace Kartowka.Packs.Core.Models;

public class UpdatePackDto
{
    [StringLength(50,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.StringLength50)
    )]
    public string? Name { get; set; }

    public string? Description { get; set; }
}