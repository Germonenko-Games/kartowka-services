namespace Kartowka.Authorization.Core.Models;

public class TokenInfo
{
    public required string AccessToken { get; set; }

    public DateTimeOffset IssueDate { get; set; }

    public DateTimeOffset ExpireDate { get; set; }
}
