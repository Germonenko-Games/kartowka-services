namespace Kartowka.Authorization.Infrastructure.Options;

public class JwtOptions
{
    public TimeSpan TokenLifespan { get; private set; }
    
    public int TokenLifespanSeconds
    {
        get => TokenLifespan.Seconds;
        set => TokenLifespan = TimeSpan.FromSeconds(value);
    }
    
    public string Secret { get; set; } = string.Empty;
}