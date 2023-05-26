using Kartowka.Core.Models.Enums;
using Kartowka.Core.Resources;

namespace Kartowka.Core.Models;

public class User
{
    public int Id { get; set; }

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
    [StringLength(250,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "StringLength250"
    )]
    public string EmailAddress { get; set; } = string.Empty;

    public UserStatus Status { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset LastOnlineDate { get; set; }

    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "Required"
    )]
    public byte[]? PasswordHash { get; set; }

    [Required(
        AllowEmptyStrings = false,
        ErrorMessageResourceType = typeof(CoreErrorMessages),
        ErrorMessageResourceName = "Required"
    )]
    public byte[]? PasswordSalt { get; set; }
}
