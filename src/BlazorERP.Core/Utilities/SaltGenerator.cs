using System.Security.Cryptography;

namespace BlazorERP.Core.Utilities;
public static class SaltGenerator
{
    public static string GenerateSalt(int size = 32)
    {
        byte[] saltBytes = new byte[size];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        return Convert.ToBase64String(saltBytes);
    }
}
