using Kartowka.Core.Models.Enums;

namespace Kartowka.Core.Constants;

public static class MimeTypes
{
    public static readonly IReadOnlyDictionary<string, AssetType> AssetTypesMap = new Dictionary<string, AssetType>
    {
        {"image/jpeg", AssetType.Image},
        {"image/png", AssetType.Image},
        {"audio/mpeg", AssetType.Music},
        {"audio/ogg", AssetType.Music},
    };
}
