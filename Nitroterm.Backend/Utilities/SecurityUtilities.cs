using System.Security.Cryptography;
using System.Text;

namespace Nitroterm.Backend.Utilities;

public static class SecurityUtilities
{
    private const int KeySize = 64;
    private const int Iterations = 360000;
    
    public static (string, string) PasswordHash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(KeySize);
        
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password),
            salt, Iterations, HashAlgorithmName.SHA512, KeySize);
        
        return (Convert.ToHexString(hash), Convert.ToHexString(salt));
    }

    public static bool VerifyPassword(string password, string hash, string salt)
    {
        byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromHexString(salt), Iterations, 
            HashAlgorithmName.SHA512, KeySize);
        
        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }
}