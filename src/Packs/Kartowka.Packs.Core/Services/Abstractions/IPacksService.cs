using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;
using Kartowka.Packs.Core.Models.Enums;

namespace Kartowka.Packs.Core.Services.Abstractions;

public interface IPacksService
{
    public Task<Pack> GetPackAsync(long packId, ICollection<PackProperties>? includeProperties = null);

    public Task<Pack> CreatePackAsync(CreatePackDto packDto);

    public Task<Pack> UpdatePackAsync(long packId, UpdatePackDto packDto);

    public Task<bool> RemovePackAsync(long packId);
}