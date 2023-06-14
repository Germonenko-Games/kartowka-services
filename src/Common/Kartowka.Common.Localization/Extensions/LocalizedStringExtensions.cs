using Microsoft.Extensions.Localization;

namespace Kartowka.Common.Localization.Extensions;

public static class LocalizedStringExtensions
{
    public static string Format(this LocalizedString localizedString, params object[] stringParameters)
        => string.Format(localizedString, stringParameters);
}