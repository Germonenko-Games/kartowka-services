using Kartowka.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Kartowka.Packs.Core.Models;

public class UpdateQuestionsCategoryDto
{
    public long? RoundId { get; set; }

    public int? Order { get; set; }

    public string? Name { get; set; }

}
