using Kartowka.Common.Blobs.Models;

namespace Kartowka.Common.Blobs;

public interface IBlobsStore
{
    public Task<List<BlobDescriptor>> GetDirectoryContentAsync(string collection, string path);

    public Task<BlobDescriptor> SaveBlobAsync(
        string collection,
        string fileName,
        Stream content,
        IDictionary<string, string>? metadata = null
    );

    public Task RemoveBlobAsync(string collection, string fileName);
}