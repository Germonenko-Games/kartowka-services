using Kartowka.Core.Resources;

namespace Kartowka.Core.Models;

public class Round
{
    public long Id { get; set; }

    [Range(0, int.MaxValue,
        ErrorMessageResourceType = typeof(ErrorMessages),
        ErrorMessageResourceName = "PositiveNumber"
    )]
    public int Order { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(50,
        ErrorMessageResourceType = typeof(ErrorMessages),
        ErrorMessageResourceName = "StringLength50"
    )]
    public string Name { get; set; } = string.Empty;

    public List<QuestionsCategory>? Categories { get; set; }
}