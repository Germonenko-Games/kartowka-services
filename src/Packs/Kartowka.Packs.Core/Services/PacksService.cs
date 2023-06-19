using Kartowka.Common.Validation;
using Kartowka.Core;
using Kartowka.Core.Exceptions;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;
using Kartowka.Packs.Core.Models.Enums;
using Kartowka.Packs.Core.Resources;
using Kartowka.Packs.Core.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Kartowka.Packs.Core.Services;

public class PacksService : IPacksService
{
    private readonly CoreContext _context;

    private readonly IAsyncValidatorsRunner<Pack> _packValidatorsRunner;

    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    public PacksService(
        CoreContext context,
        IAsyncValidatorsRunner<Pack> packValidatorsRunner,
        IStringLocalizer<PacksErrorMessages> stringLocalizer
    )
    {
        _context = context;
        _packValidatorsRunner = packValidatorsRunner;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<Pack> GetPackAsync(long packId, ICollection<PackProperties>? includeProperties = null)
    {
        var packQuery = _context.Packs.Where(p => p.Id == packId);
        if (includeProperties is not null && includeProperties.Any())
        {
            packQuery = packQuery.AsSplitQuery();

            if (includeProperties.Contains(PackProperties.Rounds))
            {
                packQuery = packQuery.Include(pack => pack.Rounds);
            }

            if (includeProperties.Contains(PackProperties.Questions))
            {
                packQuery = packQuery.Include(pack => pack.Questions);
            }

            if (includeProperties.Contains(PackProperties.Categories))
            {
                packQuery = packQuery.Include(pack => pack.QuestionsCategories);
            }
        }

        var pack = await packQuery.FirstOrDefaultAsync();
        if (pack is null)
        {
            var message = _stringLocalizer[nameof(PacksErrorMessages.PackNotFound)];
            throw new KartowkaNotFoundException(message);
        }

        return pack;
    }

    public async Task<Pack> CreatePackAsync(CreatePackDto packDto)
    {
        var now = DateTimeOffset.UtcNow;
        var pack = new Pack
        {
            AuthorId = packDto.AuthorId,
            Name = packDto.Name,
            CreatedDate = now,
            UpdatedDate = now,
        };

        await EnsurePackIsValid(pack);

        _context.Packs.Add(pack);
        await _context.SaveChangesAsync();

        return pack;
    }

    public async Task<Pack> UpdatePackAsync(long packId, UpdatePackDto packDto)
    {
        var pack = await _context.Packs.FirstOrDefaultAsync(p => p.Id == packId);
        if (pack is null)
        {
            var message = _stringLocalizer[nameof(PacksErrorMessages.PackNotFound)];
            throw new KartowkaNotFoundException(message);
        }

        if (packDto.Name is not null)
        {
            pack.Name = packDto.Name;
        }

        await EnsurePackIsValid(pack);

        await _context.SaveChangesAsync();
        return pack;
    }

    public async Task<bool> RemovePackAsync(long packId)
    {
        var pack = await _context.Packs.FindAsync(packId);
        if (pack is null)
        {
            return false;
        }

        // Related tables have OnDelete behavior set to Cascade
        // So there's no need to manually remove them
        _context.Packs.Remove(pack);
        await _context.SaveChangesAsync();

        return true;
    }

    private async Task EnsurePackIsValid(Pack pack)
    {
        var message = _stringLocalizer[nameof(PacksErrorMessages.PackDataIsInvalid)];
        await _packValidatorsRunner.EnsureValidAsync(pack, message);
    }
}