using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;

namespace Kartowka.Packs.Core.Extensions;

public static class ModelsMappingExtensions
{
    public static Round ToRound(this CreateRoundDto roundDto)
    {
        return new()
        {
            Order = roundDto.Order,
            Name = roundDto.Name,
        };
    }

    public static QuestionsCategory ToQuestionsCategory(this CreateQuestionsCategoryDto questionDto)
    {
        return new()
        {
            Name = questionDto.Name,
            Order = questionDto.Order,
            RoundId = questionDto.RoundId,
        };
    }

    public static void CopyFrom(this Round round, UpdateRoundDto roundDto)
    {
        if (roundDto.Order is not null)
        {
            round.Order = roundDto.Order.Value;
        }

        if (roundDto.Name is not null)
        {
            round.Name = roundDto.Name;
        }
    }

    public static void CopyFrom(this QuestionsCategory questionsCategory, UpdateQuestionsCategoryDto questionsCategoryDto)
    {
        if (questionsCategoryDto.RoundId is not null)
        {
            // Reset round if it's set to 0
            questionsCategory.RoundId = questionsCategoryDto.RoundId == 0
                ? null
                : questionsCategoryDto.RoundId.Value;
        }

        if (questionsCategoryDto.Name is not null)
        {
            questionsCategory.Name = questionsCategoryDto.Name;
        }

        if (questionsCategoryDto.Order is not null)
        {
            questionsCategory.Order = questionsCategoryDto.Order.Value;
        }
    }

    public static void CopyFrom(this Question question, UpdateQuestionDto questionDto)
    {
        if (questionDto.QuestionsCategoryId is not null)
        {
            // Reset question category if it's to 0
            question.QuestionsCategoryId = questionDto.QuestionsCategoryId == 0
                ? null
                : questionDto.QuestionsCategoryId;
        }

        if (questionDto.QuestionText is not null)
        {
            question.QuestionText = questionDto.QuestionText;
        }

        if (questionDto.ContentType is not null)
        {
            question.ContentType = questionDto.ContentType.Value;
        }

        if (questionDto.QuestionType is not null)
        {
            question.QuestionType = questionDto.QuestionType.Value;
        }

        if (questionDto.Score is not null)
        {
            question.Score = questionDto.Score.Value;
        }

        if (questionDto.Answer is not null)
        {
            question.Answer = questionDto.Answer;
        }
    }
}