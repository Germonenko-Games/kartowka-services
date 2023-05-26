using Kartowka.Core.Models.Enums;
using Kartowka.Core.Resources;

namespace Kartowka.Core.Models;

public class Question
{
    public long Id { get; set; }

    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(ErrorMessages),
        ErrorMessageResourceName = "Required"
    )]
    [StringLength(400,
        ErrorMessageResourceType = typeof(ErrorMessages),
        ErrorMessageResourceName = "StringLength400"
    )]
    public string Content { get; set; } = string.Empty;

    [Range(0, int.MaxValue,
        ErrorMessageResourceType = typeof(ErrorMessages),
        ErrorMessageResourceName = "PositiveNumber"
    )]
    public int Score { get; set; }

    public QuestionContentType ContentType { get; set; }

    public QuestionType QuestionType { get; set; }
}