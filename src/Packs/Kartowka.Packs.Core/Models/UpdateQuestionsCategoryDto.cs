using Kartowka.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Kartowka.Packs.Core.Models;

public class UpdateQuestionsCategoryDto
{
    public long? RoundId { get; set; }

    public int? Order { get; set; }

    [StringLength(50,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "StringLength50"
    )]
    public string? Name { get; set; }

}
