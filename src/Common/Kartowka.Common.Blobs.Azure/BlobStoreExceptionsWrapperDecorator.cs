using Azure;
using Kartowka.Common.Blobs.Models;
using Kartowka.Core.Exceptions;

namespace Kartowka.Common.Blobs.Azure;

public class BlobStoreExceptionsWrapperDecorator : IBlobsStore
{
    private readonly IBlobsStore _baseBlobStore;

    public BlobStoreExceptionsWrapperDecorator(IBlobsStore blobsStore)
    {
        _baseBlobStore = blobsStore;
    }

    public Task<BlobDescriptor> SaveBlobAsync(
        string collection,
        string fileName,
        Stream content,
        IDictionary<string, string>? metadata = null
    )
    {
        try
        {
            return _baseBlobStore.SaveBlobAsync(collection, fileName, content, metadata);
        }
        catch (RequestFailedException e)
        {
            throw new KartowkaInfrastructureException("Failed to save blob on the azure blob storage.", e);
        }
    }

    public async Task RemoveBlobAsync(string containerName, string fileName)
    {
        try
        {
            await _baseBlobStore.RemoveBlobAsync(containerName, fileName);
        }
        catch (RequestFailedException e)
        {
            throw new KartowkaInfrastructureException("Failed to remove blob from the azure blob storage.", e);
        }
    }
}
