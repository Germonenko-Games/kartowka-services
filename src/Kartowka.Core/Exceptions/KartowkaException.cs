namespace Kartowka.Core.Exceptions;

public class KartowkaException  : Exception
{
    public KartowkaException()
    {
    }
    
    public KartowkaException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}