﻿using System.Text.Json.Serialization;
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
        ErrorMessageResourceName = nameof(CoreErrorMessages.Required)
    )]
    [StringLength(50,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.StringLength50)
    )]
    public string Name { get; set; } = string.Empty;

    [Required(
        AllowEmptyStrings = true,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = nameof(CoreErrorMessages.Required)
    )]
    public string Description { get; set; } = string.Empty;

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset UpdatedDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Asset>? Assets { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<QuestionsCategory>? QuestionsCategories { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Question>? Questions { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Round>? Rounds { get; set; }
}