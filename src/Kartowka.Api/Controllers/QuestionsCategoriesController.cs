using Kartowka.Api.Models;
using Kartowka.Core.Models;
using Kartowka.Packs.Core.Models;
using Kartowka.Packs.Core.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace Kartowka.Api.Controllers;

[ApiController, Authorize, Route("api/questions-categories")]
public class QuestionsCategoriesController : ControllerBase
{
    private readonly IQuestionsCategoriesService _questionsCategoriesService;

    public QuestionsCategoriesController(IQuestionsCategoriesService questionsCategoriesService)
    {
        _questionsCategoriesService = questionsCategoriesService;
    }

    [HttpPost("")]
    [SwaggerOperation("Create a new question")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "New questions category object.",
        typeof(QuestionsCategory),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Validation error.",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<QuestionsCategory>> CreateQuestionsCategoryAsync(
        [FromBody, SwaggerRequestBody] CreateQuestionsCategoryDto categoryDto
    )
    {
        var category = await _questionsCategoriesService.CreateCategoryAsync(categoryDto);
        return Ok(category);
    }

    [HttpPatch("{categoryId:long}")]
    [SwaggerOperation("Create a new questions category")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Updated questions category object.",
        typeof(QuestionsCategory),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Validation error.",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Not found error.",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<QuestionsCategory>> CreateQuestionsCategoryAsync(
        [FromRoute] long categoryId,
        [FromBody, SwaggerRequestBody] UpdateQuestionsCategoryDto categoryDto
    )
    {
        var category = await _questionsCategoriesService.UpdateCategoryAsync(categoryId, categoryDto);
        return Ok(category);
    }

    [HttpDelete("{categoryId:long}")]
    [SwaggerOperation("Remove a category")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "No content success response.")]
    public async Task<NoContentResult> RemoveQuestionsCategoryAsync([FromRoute] long categoryId)
    {
        await _questionsCategoriesService.RemoveCategoryAsync(categoryId);
        return NoContent();
    }
}
