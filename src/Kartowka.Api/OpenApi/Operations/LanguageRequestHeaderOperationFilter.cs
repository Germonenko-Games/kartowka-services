using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Kartowka.Api.OpenApi.Operations;

public class LanguageRequestHeaderOperationFilter : IOperationFilter
{
    private const string ParameterDescription =
        "Culture ISO code. Supported languages are en, ru. " +
        "If nothing is specified, English will be used by default.";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var contentLanguageParameter = new OpenApiParameter
        {
            Description = ParameterDescription,
            Example = new OpenApiString("ru"),
            In = ParameterLocation.Header,
            Name = "Accept-Language",
            Required = false,
            Schema = new OpenApiSchema
            {
                Title = "Language and Culture",
                Type = "string"
            },
        };
        operation.Parameters.Add(contentLanguageParameter);
    }
}
