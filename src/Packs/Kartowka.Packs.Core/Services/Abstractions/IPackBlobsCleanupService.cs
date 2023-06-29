namespace Kartowka.Packs.Core.Services.Abstractions;

public interface IPackBlobsCleanupService
{
    public Task CleanupAsync(long packId, CancellationToken cancellationToken);
}