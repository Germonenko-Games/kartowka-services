using Kartowka.Common.Messaging;
using Kartowka.Common.Messaging.Models;
using Kartowka.Packs.Core.Models;
using Kartowka.Packs.Core.Services.Abstractions;

namespace Kartowka.Packs.Core.Consumers;

public class PackBlobsCleanupConsumer : IConsumer<PackCleanupMessage>
{
    private readonly IPackBlobsCleanupService _packBlobsCleanupService;

    public PackBlobsCleanupConsumer(IPackBlobsCleanupService packBlobsCleanupService)
    {
        _packBlobsCleanupService = packBlobsCleanupService;
    }

    public Task ConsumeAsync(ConsumeContext<PackCleanupMessage> model, CancellationToken cancellationToken)
    {
        return _packBlobsCleanupService.CleanupAsync(model.Body.PackId, cancellationToken);
    }
}
