using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;

namespace Api.PasswordHashers;

public class Argon2PasswordHasher : IPasswordHasher
{   
    private const int MemorySize = 19456; // 19 MiB (in KiB)
    private const int Iterations = 2;
    private const int DegreeOfParallelism = 1;
    private const int HashByteLength = 32;
    private const int SaltByteLength = 16;

    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

    public string HashPassword(string password)
    {   
        var salt = new byte[SaltByteLength];
        Rng.GetBytes(salt);
        
        return Argon2.Hash(new Argon2Config
        {
            Type = Argon2Type.DataIndependentAddressing,
            Version = Argon2Version.Nineteen,
            TimeCost = Iterations,
            MemoryCost = MemorySize,
            Lanes = DegreeOfParallelism,
            Threads = DegreeOfParallelism,
            Password = Encoding.UTF8.GetBytes(password),
            Salt = salt,
            HashLength = HashByteLength
        });
    }

    public bool Verify(string hashedPassword, string password)
    {   
        return Argon2.Verify(hashedPassword, password);
    }
}
