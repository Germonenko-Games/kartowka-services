using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;

namespace Kartowka.Packs.Core.Services.Abstractions;

public interface IRoundsService
{
    public Task<Round> CreateRoundAsync(CreateRoundDto roundDto);

    public Task<Round> UpdateRoundAsync(long roundId, UpdateRoundDto roundDto);

    public Task<bool> RemoveRoundAsync(long roundId);
}