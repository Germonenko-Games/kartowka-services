using Kartowka.Core.Resources;

namespace Kartowka.Core.Models;

public class QuestionsCategory
{
    public long Id { get; set; }

    public long? RoundId { get; set; }

    [Range(0, int.MaxValue,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "PositiveNumber"
    )]
    public int Order { get; set; }

    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "Required"
    )]
    [StringLength(50,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "StringLength50"
    )]
    public string Name { get; set; } = string.Empty;
}