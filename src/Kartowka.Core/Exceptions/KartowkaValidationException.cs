namespace Kartowka.Core.Exceptions;

public class KartowkaValidationException : KartowkaException
{
    public List<ValidationResult> Errors { get; set; } = new();

    public KartowkaValidationException()
    {
    }

    public KartowkaValidationException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}