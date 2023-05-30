using System.ComponentModel.DataAnnotations;
using Kartowka.Core.Resources;
using Kartowka.Registration.Core.Annotations;
using Kartowka.Registration.Core.Resources;

namespace Kartowka.Registration.Core.Models;

public class UserData
{
    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "Required"
    )]
    [StringLength(250,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "StringLength250"
    )]
    public string EmailAddress { get; set; } = string.Empty;

    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "Required"
    )]
    [StringLength(50,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "StringLength50"
    )]
    public string Username { get; set; } = string.Empty;

    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "Required"
    )]
    [ComplexPassword(
        ErrorMessageResourceType = typeof(ErrorMessages), 
        ErrorMessageResourceName = "PasswordIsInvalid"
    )]
    public string Password { get; set; } = string.Empty;
}