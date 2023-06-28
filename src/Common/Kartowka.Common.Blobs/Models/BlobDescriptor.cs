namespace Kartowka.Common.Blobs.Models;

public record struct BlobDescriptor
{
    public string Name;

    public Uri BlobUri;

    public long Size;

    public ICollection<KeyValuePair<string, string>>? Metadata;
}