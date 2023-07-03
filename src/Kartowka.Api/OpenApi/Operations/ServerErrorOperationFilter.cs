using Kartowka.Api.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net.Mime;

namespace Kartowka.Api.OpenApi.Operations;

public class ServerErrorOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!context.SchemaRepository.TryLookupByType(typeof(ErrorResponse), out var schema))
        {
            return;
        }

        var serverErrorResponse = new OpenApiResponse
        {
            Description = "Unhandled server error",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                {
                    MediaTypeNames.Application.Json,
                    new OpenApiMediaType
                    {
                        Schema = schema
                    }
                }
            },
        };

        operation.Responses.Add(
            StatusCodes.Status500InternalServerError.ToString(),
            serverErrorResponse
        );
    }
}
