using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;

namespace Kartowka.Packs.Core.Services.Abstractions;

public interface IQuestionsService
{
    public Task<Question> GetQuestionAsync(long questionId);

    public Task<Question> CreateQuestionAsync(CreateQuestionDto questionDto);

    public Task<Question> UpdateQuestionAsync(long questionId, UpdateQuestionDto questionDto);

    public Task<bool> RemoveQuestionAsync(long questionId);
}