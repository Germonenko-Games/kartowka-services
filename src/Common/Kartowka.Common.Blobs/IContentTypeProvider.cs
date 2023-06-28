namespace Kartowka.Common.Blobs;

public interface IContentTypeProvider
{
    public IReadOnlyDictionary<string, string> Mappings { get; }

    public bool TryGetContentType(string fileExtension, out string contentType);

    public bool TryGetFileExtension(string contentType, out string fileExtension);
}