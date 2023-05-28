using System.Text;
using Kartowka.Api.Models;
using Kartowka.Api.Resources;
using Kartowka.Core.Exceptions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Localization;

namespace Kartowka.Api.Middleware;

public class KartowkaExceptionsHandlingMiddleware : IMiddleware
{
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
        catch (KartowkaNotFoundException e)
        {
            var body = await context.Request.BodyReader.ReadAsync();
            _logger.LogInformation(
                "Not Found (404) returned from {Url} trying to handle a request with the following body: {Body}",
                context.Request.GetDisplayUrl(),
                Encoding.UTF8.GetString(body.Buffer)
            );

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new ErrorResponse(e.Message));
        }
        catch (Exception e)
        {
            var body = await context.Request.BodyReader.ReadAsync();
            _logger.LogError(
                e,
                "Server Error (500) returned from {Url} trying to handle a request with the following body: {Body}",
                context.Request.GetDisplayUrl(),
                Encoding.UTF8.GetString(body.Buffer)
            );

            var response = new ErrorResponse(_stringLocalizer["UnknownErrorResponse"]);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}