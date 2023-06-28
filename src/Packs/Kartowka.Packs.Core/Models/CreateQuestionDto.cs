using System.ComponentModel.DataAnnotations;
using Kartowka.Core.Models.Enums;
using Kartowka.Core.Resources;

namespace Kartowka.Packs.Core.Models;

public class CreateQuestionDto
{
    public long PackId { get; set; }

    public long? QuestionsCategoryId { get; set; }

    [StringLength(400,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.StringLength400)
    )]
    public string? QuestionText { get; set; }

    public long? AssetId { get; set; }

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
}