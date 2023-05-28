namespace Kartowka.Common.Crypto.Abstractions;

public interface IHasher
{
    public (byte[] Hash, byte[] Salt) Hash(string input);

    public byte[] Hash(string input, byte[] salt);
}
