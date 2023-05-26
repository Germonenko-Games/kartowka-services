using Kartowka.Core.Models.Enums;
using Kartowka.Core.Resources;

namespace Kartowka.Core.Models;

public class Pack
{
    public long Id { get; set; }

    public int AuthorId { get; set; }

    public PackStatus Status { get; set; }

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

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset UpdatedDate { get; set; }

    public List<Round>? Rounds { get; set; }
}