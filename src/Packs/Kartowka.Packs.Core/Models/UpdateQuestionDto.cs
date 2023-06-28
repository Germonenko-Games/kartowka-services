using System.ComponentModel.DataAnnotations;
using Kartowka.Core.Models.Enums;
using Kartowka.Core.Resources;

namespace Kartowka.Packs.Core.Models;

public class UpdateQuestionDto
{
    public long? QuestionsCategoryId { get; set; }

    [StringLength(400,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.StringLength400)
    )]
    public string? QuestionText { get; set; }

    public long? AssetId { get; set; }

    public int? Score { get; set; }

    [StringLength(400,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.StringLength400)
    )]
    public string? Answer { get; set; }

    public QuestionContentType? ContentType { get; set; }

    public QuestionType? QuestionType { get; set; }
}