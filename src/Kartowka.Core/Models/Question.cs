using Kartowka.Core.Models.Enums;
using Kartowka.Core.Resources;

namespace Kartowka.Core.Models;

public class Question
{
    public long Id { get; set; }

    public long? QuestionsCategoryId { get; set; }

    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "Required"
    )]
    [StringLength(400,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "StringLength400"
    )]
    public string Content { get; set; } = string.Empty;

    [Range(0, int.MaxValue,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "PositiveNumber"
    )]
    public int Score { get; set; }

    public QuestionContentType ContentType { get; set; }

    public QuestionType QuestionType { get; set; }
}