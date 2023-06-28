using Kartowka.Common.Blobs.Models;

namespace Kartowka.Common.Blobs;

public interface IBlobsStore
{
    public Task<BlobDescriptor> SaveBlobAsync(
        string collection,
        string fileName,
        Stream content,
        IDictionary<string, string>? metadata = null
    );

    public Task RemoveBlobAsync(string containereName, string fileName);
}