using Kartowka.Common.Blobs;
using Kartowka.Common.Blobs.Models;
using Kartowka.Core;
using Kartowka.Core.Exceptions;
using Kartowka.Core.Models;
using Kartowka.Core.Models.Enums;
using Kartowka.Packs.Core.Constants;
using Kartowka.Packs.Core.Models;
using Kartowka.Packs.Core.Options;
using Kartowka.Packs.Core.Resources;
using Kartowka.Packs.Core.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Kartowka.Packs.Core.Services;

public class AssetsService : IAssetsService
{
    private readonly CoreContext _context;

    private readonly IBlobsStore _blobsStore;

    private readonly IContentTypeProvider _contentTypeProvider;

    private readonly IOptionsSnapshot<PacksOptions> _options;

    private readonly IStringLocalizer<PacksErrorMessages> _errorMessages;

    private readonly IStringLocalizer<Assets> _assetsStrings;

    public AssetsService(
        CoreContext context,
        IBlobsStore blobsStore,
        IContentTypeProvider contentTypeProvider,
        IOptionsSnapshot<PacksOptions> options,
        IStringLocalizer<PacksErrorMessages> errorMessages,
        IStringLocalizer<Assets> assetsStrings
    )
    {
        _context = context;
        _blobsStore = blobsStore;
        _contentTypeProvider = contentTypeProvider;
        _options = options;
        _errorMessages = errorMessages;
        _assetsStrings = assetsStrings;
    }

    public async Task<Asset> GetAssetAsync(long assetId)
    {
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset is null)
        {
            var message = _errorMessages.GetString(nameof(PacksErrorMessages.AssetNotFound));
            throw new KartowkaNotFoundException(message);
        }

        return asset;
    }

    public async Task<Asset> CreateAssetAsync(UploadAssetDto assetDto)
    {
        var pack = await _context.Packs.FirstOrDefaultAsync(p => p.Id == assetDto.PackId);
        if (pack is null)
        {
            var message = _errorMessages[nameof(PacksErrorMessages.PackNotFound)];
            throw new KartowkaNotFoundException(message);
        }

        var storageInUse = await _context.Packs
            .Where(p => p.AuthorId == pack.AuthorId)
            .Select(p => p.Assets!.Sum(a => a.Size))
            .SumAsync();

        var newAssetSize = assetDto.Content.Length;

        if (storageInUse + newAssetSize > _options.Value.DefaultPlanStorageLimitMb * 1024 * 1024)
        {
            var message = _errorMessages[nameof(PacksErrorMessages.AssetsStorageLimitExceeded)];
            throw new KartowkaException(message);
        }

        if (!_contentTypeProvider.TryGetFileExtension(assetDto.MimeType, out var fileExtension))
        {
            var message = _errorMessages[nameof(PacksErrorMessages.AssetMimeTypeIsUnsupported)];
            throw new KartowkaException(message);
        }

        var mimeCategory = assetDto.MimeType[..assetDto.MimeType.IndexOf('/')];
        var assetType = mimeCategory switch
        {
            "audio" => AssetType.Music,
            "image" => AssetType.Image,
            _ => throw new KartowkaException(
                _errorMessages.GetString(nameof(PacksErrorMessages.AssetMimeTypeIsUnsupported))
            ),
        };

        var systemFileName = $"pack-{pack.Id}/{Guid.NewGuid()}.{fileExtension}";

        var blob = await _blobsStore.SaveBlobAsync(
            BlobsCollectionNames.Assets,
            systemFileName,
            assetDto.Content
        );

        var asset = new Asset
        {
            AssetType = assetType,
            BlobUrl = blob.BlobUri.ToString(),
            DisplayName = assetDto.DisplayName ?? GetNewFileName(),
            Size = blob.Size,
            SystemName = systemFileName,
        };

        pack.Assets ??= new List<Asset>();
        pack.Assets.Add(asset);
        
        await _context.SaveChangesAsync();

        return asset;
    }

    public async Task<Asset> UpdateAssetDetailsAsync(long assetId, UpdateAssetDetailsDto assetDto)
    {
        var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == assetId);
        if (asset is null)
        {
            var message = _errorMessages[nameof(PacksErrorMessages.PackNotFound)];
            throw new KartowkaNotFoundException(message);
        }

        if (assetDto.DisplayName is not null)
        {
            asset.DisplayName = assetDto.DisplayName;
        }

        await _context.SaveChangesAsync();

        return asset;
    }

    public async Task<bool> RemoveAssetAsync(long assetId)
    {
        var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == assetId);
        if (asset is null)
        {
            return false;
        }

        await _blobsStore.RemoveBlobAsync(BlobsCollectionNames.Assets, asset.SystemName);

        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();

        return true;
    }

    private string GetNewFileName() => _assetsStrings[nameof(Assets.NewFile)];
}