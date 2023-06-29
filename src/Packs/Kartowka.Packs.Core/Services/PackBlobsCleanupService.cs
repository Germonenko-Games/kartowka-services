using Kartowka.Common.Blobs;
using Kartowka.Packs.Core.Constants;
using Kartowka.Packs.Core.Services.Abstractions;

namespace Kartowka.Packs.Core.Services;

public class PackBlobsCleanupService : IPackBlobsCleanupService
{
    private readonly IBlobsStore _blobsStore;

    public PackBlobsCleanupService(IBlobsStore blobsStore)
    {
        _blobsStore = blobsStore;
    }

    public async Task CleanupAsync(long packId, CancellationToken cancellationToken)
    {

        var packBlobDescriptors = await _blobsStore.GetDirectoryContentAsync(
            BlobsCollectionNames.Assets,
            $"pack-{packId}"
        );

        foreach(var blob in packBlobDescriptors)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await _blobsStore.RemoveBlobAsync(BlobsCollectionNames.Assets, blob.Name);
        }
    }
}
