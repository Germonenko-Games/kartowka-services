using Kartowka.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Kartowka.Authorization.Core.Models;

public class UserCredentials
{
    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "Required"
    )]
    public string? EmailAddress { get; set; }

    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "Required"
    )]
    public string? Password { get; set; }
}