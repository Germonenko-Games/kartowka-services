using System.Net.Mime;
using Kartowka.Api.Models;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;
using Kartowka.Packs.Core.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Kartowka.Api.Controllers;

[ApiController, Authorize, Route("api/questions"), Consumes(MediaTypeNames.Application.Json)]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionsService _questionsService;

    public QuestionsController(IQuestionsService questionsService)
    {
        _questionsService = questionsService;
    }

    [HttpPost("")]
    [SwaggerOperation("Creates a new question")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "New question object",
        typeof(Question),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Question validation error",
        typeof(Question),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<Question>> CreateQuestionAsync(
        [FromBody, SwaggerRequestBody] CreateQuestionDto questionDto
    )
    {
        var question = await _questionsService.CreateQuestionAsync(questionDto);
        return Ok(question);
    }

    [HttpGet("{questionId:long}")]
    [SwaggerOperation("Get question by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Question model", typeof(Question), MediaTypeNames.Application.Json)]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Not found error",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<Question>> GetQuestionAsync([FromRoute] long questionId)
    {
        var question = await _questionsService.GetQuestionAsync(questionId);
        return Ok(question);
    }

    [HttpPatch("{questionId:long}")]
    [SwaggerOperation("Update a question")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Updated question object",
        typeof(Question),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Question validation error",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Question not found error",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<Question>> UpdateQuestionAsync(
        [FromRoute] long questionId,
        [FromBody, SwaggerRequestBody] UpdateQuestionDto questionDto
    )
    {
        var question = await _questionsService.UpdateQuestionAsync(questionId, questionDto);
        return Ok(question);
    }

    [HttpDelete("{questionId:long}")]
    [SwaggerOperation("Removes a question", "This endpoint is idempotent.")]
    public async Task<NoContentResult> RemoveQuestionAsync([FromRoute] long questionId)
    {
        await _questionsService.RemoveQuestionAsync(questionId);
        return NoContent();
    }
}