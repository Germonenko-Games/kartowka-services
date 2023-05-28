namespace Kartowka.Core.Exceptions;

public class KartowkaNotFoundException : KartowkaException
{
    public KartowkaNotFoundException()
    {
    }

    public KartowkaNotFoundException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}