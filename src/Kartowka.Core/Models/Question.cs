using System.Text.Json.Serialization;
using Kartowka.Core.Models.Enums;
using Kartowka.Core.Resources;

namespace Kartowka.Core.Models;

public class Question
{
    public long Id { get; set; }

    public long? QuestionsCategoryId { get; set; }

    [StringLength(400,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.StringLength400)
    )]
    public string? QuestionText { get; set; } = string.Empty;

    [Range(0, int.MaxValue,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.PositiveNumber)
    )]
    public int Score { get; set; }

    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.Required)
    )]
    [StringLength(400,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.StringLength400)
    )]
    public string Answer { get; set;} = string.Empty;

    public QuestionContentType ContentType { get; set; }

    public QuestionType QuestionType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Asset? Asset { get; set; }
}