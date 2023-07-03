using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;

namespace Kartowka.Packs.Core.Services.Abstractions;

public interface IAssetsService
{
    public Task<Asset> GetAssetAsync(long assetId);

    public Task<Asset> CreateAssetAsync(UploadAssetDto assetDto);

    public Task<Asset> UpdateAssetDetailsAsync(long assetId, UpdateAssetDetailsDto assetDto);

    public Task<bool> RemoveAssetAsync(long assetId);
}