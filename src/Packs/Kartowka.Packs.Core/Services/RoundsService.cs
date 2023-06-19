using Kartowka.Common.Validation;
using Kartowka.Core;
using Kartowka.Core.Exceptions;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Extensions;
using Kartowka.Packs.Core.Models;
using Kartowka.Packs.Core.Resources;
using Kartowka.Packs.Core.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Kartowka.Packs.Core.Services;

public class RoundsService : IRoundsService
{
    private readonly CoreContext _context;

    private readonly IAsyncValidatorsRunner<Pack> _packValidatorsRunner;

    private readonly IAsyncValidatorsRunner<Round> _roundValidatorsRunner;

    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    public RoundsService(
        CoreContext context,
        IAsyncValidatorsRunner<Pack> packValidatorsRunner,
        IAsyncValidatorsRunner<Round> roundValidatorsRunner,
        IStringLocalizer<PacksErrorMessages> stringLocalizer
    )
    {
        _context = context;
        _packValidatorsRunner = packValidatorsRunner;
        _roundValidatorsRunner = roundValidatorsRunner;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<Round> CreateRoundAsync(CreateRoundDto roundDto)
    {
        var pack = await _context.Packs
            .AsSplitQuery()
            .Include(p => p.Rounds)
            .FirstOrDefaultAsync(p => p.Id == roundDto.PackId);

        if (pack is null)
        {
            var message = _stringLocalizer[nameof(PacksErrorMessages.PackNotFound)];
            throw new KartowkaNotFoundException(message);
        }

        var round = roundDto.ToRound();
        pack.Rounds ??= new List<Round>();
        pack.Rounds.Add(round);

        await EnsurePackIsValid(pack);
        await EnsureRoundIsValid(round);

        return round;
    }

    public async Task<Round> UpdateRoundAsync(long roundId, UpdateRoundDto roundDto)
    {
        var round = await _context.Rounds.FirstOrDefaultAsync(r => r.Id == roundId);
        if (round is null)
        {
            var message = _stringLocalizer[nameof(PacksErrorMessages.RoundNotFound)];
            throw new KartowkaNotFoundException(message);
        }

        round.CopyFrom(roundDto);
        await EnsureRoundIsValid(round);

        await _context.SaveChangesAsync();
        return round;
    }

    public async Task<bool> RemoveRoundAsync(long roundId)
    {
        var round = await _context.Rounds.FindAsync(roundId);
        if (round is null)
        {
            return false;
        }

        // Related tables have OnDelete behaviour set to SetNull
        // So there's no need to manually update them
        _context.Rounds.Remove(round);
        await _context.SaveChangesAsync();

        return true;
    }

    private async Task EnsurePackIsValid(Pack pack)
    {
        var message = _stringLocalizer[nameof(PacksErrorMessages.PackDataIsInvalid)];
        await _packValidatorsRunner.EnsureValidAsync(pack, message);
    }

    private async Task EnsureRoundIsValid(Round round)
    {
        var message = _stringLocalizer[nameof(PacksErrorMessages.RoundDataIsInvalid)];
        await _roundValidatorsRunner.EnsureValidAsync(round, message);
    }
}