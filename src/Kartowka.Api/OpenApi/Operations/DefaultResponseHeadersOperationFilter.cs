using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Kartowka.Api.OpenApi.Operations;

public class DefaultResponseHeadersOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var headerSchema = new OpenApiSchema
        {
            Type = "string"
        };

        var languageHeader = new OpenApiHeader
        {
            Description = "Response language",
            Example = new OpenApiString("ru"),
            Schema = headerSchema,
        };

        var serverHeader = new OpenApiHeader
        {
            Description = "Contains information about how the server handles requests",
            Example = new OpenApiString("Kestrel"),
            Schema = headerSchema,
        };

        var dateHeader = new OpenApiHeader
        {
            Description = "An entity header indicating the intended language audience",
            Example = new OpenApiString("Kestrel"),
            Schema = headerSchema,
        };

        var transferEncoding = new OpenApiHeader
        {
            Description = "Specifies the form of encoding used to transfer an entity yo a user",
            Example = new OpenApiString("Kestrel"),
            Schema = headerSchema,
        };

        foreach (var (_, responseDefinition) in operation.Responses)
        {
            responseDefinition.Headers["Content-Language"] = languageHeader;
            responseDefinition.Headers["Date"] = dateHeader;
            responseDefinition.Headers["Server"] = serverHeader;
            responseDefinition.Headers["Transfer-Encoding"] = transferEncoding;
        }
    }
}
