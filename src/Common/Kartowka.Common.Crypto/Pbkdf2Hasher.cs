using System.Security.Cryptography;
using Kartowka.Common.Crypto.Abstractions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Kartowka.Common.Crypto;

public class Pbkdf2Hasher : IHasher
{
    private const int IterationsCount = 10_000;

    private const int RequestBytesLength = 128 / 8;

    private const int DefaultSaltLength = 128 / 8;

    public byte[] Hash(string source, byte[] salt)
        => KeyDerivation.Pbkdf2(
            source,
            salt,
            KeyDerivationPrf.HMACSHA256,
            IterationsCount,
            RequestBytesLength
        );

    public (byte[] Hash, byte[] Salt) Hash(string source)
    {
        var salt = RandomNumberGenerator.GetBytes(DefaultSaltLength);
        return (Hash(source, salt), salt);
    }
}