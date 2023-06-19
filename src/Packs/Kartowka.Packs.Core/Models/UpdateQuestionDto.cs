using Kartowka.Core.Models.Enums;

namespace Kartowka.Packs.Core.Models;

public class UpdateQuestionDto
{
    public long? QuestionsCategoryId { get; set; }

    public string? Content { get; set; }

    public int? Score { get; set; }

    public string? Answer { get; set; }

    public QuestionContentType? ContentType { get; set; }

    public QuestionType? QuestionType { get; set; }
}