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

public class QuestionsService : IQuestionsService
{
    private readonly CoreContext _context;

    private readonly IValidatorsRunner<Question> _questionValidatorsRunner;

    private readonly IValidatorsRunner<Pack> _packValidatorsRunner;

    private readonly IStringLocalizer<PacksErrorMessages> _stringLocalizer;

    public QuestionsService(
        CoreContext context,
        IValidatorsRunner<Question> questionValidatorsRunner,
        IValidatorsRunner<Pack> packValidatorsRunner,
        IStringLocalizer<PacksErrorMessages> stringLocalizer
    )
    {
        _context = context;
        _questionValidatorsRunner = questionValidatorsRunner;
        _packValidatorsRunner = packValidatorsRunner;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<Question> GetQuestionAsync(long questionId)
    {
        var question = await _context.Questions.FindAsync(questionId);
        if (question is null)
        {
            var message = _stringLocalizer[nameof(PacksErrorMessages.QuestionNotFound)];
            throw new KartowkaNotFoundException(message);
        }

        return question;
    }

    public async Task<Question> CreateQuestionAsync(CreateQuestionDto questionDto)
    {
        var pack = await _context.Packs
            .Include(p => p.Questions)
            .FirstOrDefaultAsync(p => p.Id == questionDto.PackId);


        if (pack is null)
        {
            var message = _stringLocalizer[nameof(PacksErrorMessages.PackNotFound)];
            throw new KartowkaException(message);
        }

        var question = new Question
        {
            QuestionsCategoryId = questionDto.QuestionsCategoryId,
            Answer = questionDto.Answer,
            Score = questionDto.Score,
            ContentType = questionDto.ContentType,
            QuestionType = questionDto.QuestionType,
            QuestionText = questionDto.QuestionText,
        };

        if (questionDto.AssetId != null)
        {
            var asset = await _context.Assets.FindAsync(questionDto.AssetId);
            if (asset is null)
            {
                var message = _stringLocalizer.GetString(nameof(PacksErrorMessages.AssetNotFound));
                throw new KartowkaException(message);
            }

            question.Asset = asset;
        }

        pack.Questions ??= new ();
        pack.Questions.Add(question);

        await EnsureQuestionIsValid(question);
        await EnsurePackIsValid(pack);

        await _context.SaveChangesAsync();

        return question;
    }

    public async Task<Question> UpdateQuestionAsync(long questionId, UpdateQuestionDto questionDto)
    {
        var question = await _context.Questions
            .Include(q => q.Asset)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question is null)
        {
            var message = _stringLocalizer[nameof(PacksErrorMessages.QuestionNotFound)];
            throw new KartowkaNotFoundException(message);
        }

        question.CopyFrom(questionDto);

        if (questionDto.AssetId is not null && questionDto.AssetId != 0)
        {
            var asset = await _context.Assets.FindAsync(questionDto.AssetId);
            if (asset is null)
            {
                var message = _stringLocalizer.GetString(nameof(PacksErrorMessages.AssetNotFound));
                throw new KartowkaException(message);
            }

            question.Asset = asset;
        }
        else if (questionDto.AssetId == 0)
        {
            question.Asset = null;
        }

        await EnsureQuestionIsValid(question);

        await _context.SaveChangesAsync();

        return question;
    }

    public async Task<bool> RemoveQuestionAsync(long questionId)
    {
        var question = await _context.Questions.FindAsync(questionId);
        if (question is null)
        {
            return false;
        }

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task EnsurePackIsValid(Pack pack)
    {
        var message = _stringLocalizer[nameof(PacksErrorMessages.PackDataIsInvalid)];
        await _packValidatorsRunner.EnsureValidAsync(pack, message);
    }

    private async Task EnsureQuestionIsValid(Question question)
    {
        var message = _stringLocalizer[nameof(PacksErrorMessages.QuestionDataIsInvalid)];
        await _questionValidatorsRunner.EnsureValidAsync(question, message);
    }
}