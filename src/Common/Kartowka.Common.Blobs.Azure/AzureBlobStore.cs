using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Kartowka.Common.Blobs.Models;

namespace Kartowka.Common.Blobs.Azure;

public class AzureBlobStore : IBlobsStore
{
    private const string DefaultContentType = "application/octet-stream";

    private readonly BlobServiceClient _blobsServiceClient;

    private readonly IContentTypeProvider _contentTypeProvider;

    public AzureBlobStore(BlobServiceClient blobServiceClient, IContentTypeProvider contentTypeProvider)
    {
        _blobsServiceClient = blobServiceClient;
        _contentTypeProvider = contentTypeProvider;
    }

    public async Task<List<BlobDescriptor>> GetDirectoryContentAsync(string collection, string path)
    {
        var containerClient = _blobsServiceClient.GetBlobContainerClient(collection);
        var blobDescriptors = new List<BlobDescriptor>();
        await foreach (var blob in containerClient.GetBlobsAsync(prefix: path))
        {
            var blobClient = containerClient.GetBlobClient(blob.Name);

            blobDescriptors.Add(new()
            {
                Name = blob.Name,
                BlobUri = blobClient.Uri,
                Size = blob.Properties.ContentLength ?? -1,
                Metadata = blob.Metadata,
            });
        }

        return blobDescriptors;
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

        var headers = new BlobHttpHeaders
        {
            ContentType = GetContentType(fileName),
        };

        await blobClient.UploadAsync(
            content,
            httpHeaders: headers,
            metadata: metadata,
            conditions: null
        );

        if (metadata is not null && metadata.Any())
        {
            await blobClient.SetMetadataAsync(metadata);
        }

        return new BlobDescriptor
        {
            Name = blobClient.Name,
            Size = content.Length,
            BlobUri = blobClient.Uri,
            Metadata = metadata,
        };
    }

    public async Task RemoveBlobAsync(string containerName, string fileName)
    {
        var containerClient = _blobsServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync();
    }

    private string GetContentType(string fileName)
    {
        var fileExtension = Path.GetExtension(fileName)?.TrimStart('.');
        if (fileExtension is null)
        {
            return DefaultContentType;
        }

        if (_contentTypeProvider.TryGetContentType(fileExtension, out var contentType))
        {
            return contentType;
        }

        return DefaultContentType;
    }
}