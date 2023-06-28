using Azure.Storage.Blobs;
using Kartowka.Common.Blobs.Models;

namespace Kartowka.Common.Blobs.Azure;

public class AzureBlobStore : IBlobsStore
{
    private readonly BlobServiceClient _blobsServiceClient;

    public AzureBlobStore(BlobServiceClient blobServiceClient)
    {
        _blobsServiceClient = blobServiceClient;
    }

    public async Task<BlobDescriptor> SaveBlobAsync(
        string collection,
        string fileName,
        Stream content,
        IDictionary<string, string>? metadata = null
    )
    {
        var containerClient = _blobsServiceClient.GetBlobContainerClient(collection);
        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(content, true);

        if (metadata is not null && metadata.Any())
        {
            await blobClient.SetMetadataAsync(metadata);
        }

        return new BlobDescriptor
        {
            Name = blobClient.Name,
            Size = content.Length,
            BlobUri = blobClient.Uri,
        };
    }

    public async Task RemoveBlobAsync(string containerName, string fileName)
    {
        var containerClient = _blobsServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync();
    }
}