using System.Security.Cryptography;

namespace DockerHomeWorkApp.Security;

public interface IPasswordHasher
{
    (byte[] hash, byte[] salt) Hash(string password, int? iterations = null);
    bool Verify(string password, byte[] hash, byte[] salt, int? iterations = null);
}

public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int DefaultIterations = 100_000;
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public (byte[] hash, byte[] salt) Hash(string password, int? iterations = null)
    {
        var iters = iterations ?? DefaultIterations;
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iters, HashAlgorithmName.SHA256, KeySize);
        return (hash, salt);
    }

    public bool Verify(string password, byte[] hash, byte[] salt, int? iterations = null)
    {
        var iters = iterations ?? DefaultIterations;
        var computed = Rfc2898DeriveBytes.Pbkdf2(password, salt, iters, HashAlgorithmName.SHA256, hash.Length);
        return CryptographicOperations.FixedTimeEquals(hash, computed);
    }
}
