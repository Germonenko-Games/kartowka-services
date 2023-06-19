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

public class QuestionsCategoriesService : IQuestionsCategoriesService
{
    private readonly CoreContext _context;

    private readonly IAsyncValidatorsRunner<Pack> _packValidatorsRunner;

    private readonly IAsyncValidatorsRunner<QuestionsCategory> _questionsCategoryValidatorsRunner;

    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    public QuestionsCategoriesService(
        CoreContext context,
        IAsyncValidatorsRunner<Pack> packValidatorsRunner,
        IAsyncValidatorsRunner<QuestionsCategory> questionsCategoryValidatorsRunner,
        IStringLocalizer<PacksErrorMessages> stringLocalizer
    )
    {
        _context = context;
        _packValidatorsRunner = packValidatorsRunner;
        _questionsCategoryValidatorsRunner = questionsCategoryValidatorsRunner;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<QuestionsCategory> CreateCategoryAsync(CreateQuestionsCategoryDto questionsCategoryDto)
    {
        var pack = await _context.Packs
            .AsSplitQuery()
            .Include(pack => pack.QuestionsCategories)
            .FirstOrDefaultAsync(p => p.Id == questionsCategoryDto.PackId);

        if (pack is null)
        {
            var message = _stringLocalizer[nameof(PacksErrorMessages.PackNotFound)];
            throw new KartowkaNotFoundException(message);
        }

        var category = questionsCategoryDto.ToQuestionsCategory();

        pack.QuestionsCategories ??= new List<QuestionsCategory>();
        pack.QuestionsCategories.Add(category);

        await EnsurePackIsValid(pack);
        await EnsureQuestionsCategoryIsValid(category);

        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<QuestionsCategory> UpdateCategoryAsync(
        long categoryId,
        UpdateQuestionsCategoryDto categoryDto
    )
    {
        var category = await _context.QuestionsCategories.FirstOrDefaultAsync(c => c.Id == categoryId);
        if (category is null)
        {
            var message = _stringLocalizer[nameof(PacksErrorMessages.QuestionsCategoryNotFound)];
            throw new KartowkaNotFoundException(message);
        }

        category.CopyFrom(categoryDto);

        await EnsureQuestionsCategoryIsValid(category);
        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<bool> RemoveCategoryAsync(long categoryId)
    {
        var category = await _context.QuestionsCategories.FindAsync(categoryId);
        if (category is null)
        {
            return false;
        }

        _context.QuestionsCategories.Remove(category);
        await _context.SaveChangesAsync();

        return true;
    }

    private async Task EnsurePackIsValid(Pack pack)
    {
        var message = _stringLocalizer[nameof(PacksErrorMessages.PackDataIsInvalid)];
        await _packValidatorsRunner.EnsureValidAsync(pack, message);
    }

    private async Task EnsureQuestionsCategoryIsValid(QuestionsCategory category)
    {
        var message = _stringLocalizer[nameof(PacksErrorMessages.QuestionsCategoryDataIsInvalid)];
        await _questionsCategoryValidatorsRunner.EnsureValidAsync(category, message);
    }
}