using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kartowka.Api.Models;
using Kartowka.Api.Resources;
using Kartowka.Core.Exceptions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Localization;

namespace Kartowka.Api.Middleware;

public class KartowkaExceptionsHandlingMiddleware : IMiddleware
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly ILogger<KartowkaExceptionsHandlingMiddleware> _logger;

    private readonly IStringLocalizer<ErrorMessages> _stringLocalizer;

    public KartowkaExceptionsHandlingMiddleware(
        ILogger<KartowkaExceptionsHandlingMiddleware> logger,
        IStringLocalizer<ErrorMessages> stringLocalizer
    )
    {
        _logger = logger;
        _stringLocalizer = stringLocalizer;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (KartowkaValidationException e)
        {
            var body = await context.Request.BodyReader.ReadAsync();
            _logger.LogInformation(
                "Bad Request (400) returned from {Url} trying to handle a request with the following body: {Body}",
                context.Request.GetDisplayUrl(),
                Encoding.UTF8.GetString(body.Buffer)
            );

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ErrorResponse(e.Message, e.Errors), _serializerOptions);
        }
        catch (KartowkaNotFoundException e)
        {
            var body = await context.Request.BodyReader.ReadAsync();
            _logger.LogInformation(
                "Not Found (404) returned from {Url} trying to handle a request with the following body: {Body}",
                context.Request.GetDisplayUrl(),
                Encoding.UTF8.GetString(body.Buffer)
            );

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new ErrorResponse(e.Message), _serializerOptions);
        }
        catch (KartowkaException e)
        {
            var body = await context.Request.BodyReader.ReadAsync();
            _logger.LogInformation(
                "Bad Request (400) returned from {Url} trying to handle a request with the following body: {Body}",
                context.Request.GetDisplayUrl(),
                Encoding.UTF8.GetString(body.Buffer)
            );

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new ErrorResponse(e.Message), _serializerOptions);
        }
        catch (Exception e)
        {
            var logLevel = e is KartowkaInfrastructureException ? LogLevel.Critical : LogLevel.Error;

            var body = await context.Request.BodyReader.ReadAsync();
            _logger.Log(
                logLevel,
                e,
                "Server Error (500) returned from {Url} trying to handle a request with the following body: {Body}",
                context.Request.GetDisplayUrl(),
                Encoding.UTF8.GetString(body.Buffer)
            );

            var response = new ErrorResponse(_stringLocalizer[nameof(ErrorMessages.UnknownErrorResponse)]);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(response, _serializerOptions);
        }
    }
}