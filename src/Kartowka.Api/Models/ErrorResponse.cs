using System.ComponentModel.DataAnnotations;

namespace Kartowka.Api.Models;

public class ErrorResponse
{
    public string Message { get; }

    public List<ValidationResult>? Errors { get; }

    public ErrorResponse(string message, List<ValidationResult>? erors = null)
    {
        Message = message;
        Errors = erors;
    }
}