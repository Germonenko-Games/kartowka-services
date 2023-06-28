namespace Kartowka.Core.Exceptions;

public class KartowkaInfrastructureException : KartowkaException
{
    public KartowkaInfrastructureException()
    {
    }

    public KartowkaInfrastructureException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
