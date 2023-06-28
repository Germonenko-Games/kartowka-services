using System.ComponentModel.DataAnnotations;
using Kartowka.Core.Resources;

namespace Kartowka.Packs.Core.Models;

public class UpdateRoundDto
{
    public int? Order { get; set; }

    [StringLength(50,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "StringLength50"
    )]
    public string? Name { get; set; }
}