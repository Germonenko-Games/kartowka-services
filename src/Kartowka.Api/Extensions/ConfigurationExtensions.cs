namespace Kartowka.Api.Extensions;

public static class ConfigurationExtensions
{
    public static T GetRequiredValue<T>(this IConfiguration configuration, string key)
    {
        var configurationValue = configuration.GetValue<T>(key);
        return configurationValue ?? throw new Exception($"Configuration item {key} was not found.");
    }
}