using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;

namespace Kartowka.Packs.Core.Services.Abstractions;

public interface IQuestionsCategoriesService
{
    public Task<QuestionsCategory> CreateCategoryAsync(CreateQuestionsCategoryDto questionsCategoryDto);

    public Task<bool> RemoveCategoryAsync(long categoryId);

    public Task<QuestionsCategory> UpdateCategoryAsync(long categoryId, UpdateQuestionsCategoryDto categoryDto);
}