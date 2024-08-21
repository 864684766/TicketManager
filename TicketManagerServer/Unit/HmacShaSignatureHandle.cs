using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace TicketManagerServer.Unit
{
    public static class HmacShaSignatureHandle
    {

        // 生成盐
        public static byte[] GenerateSalt(int size)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // 生成派生密钥
        public static byte[] GenerateDerivedKey(string password, byte[] salt, int keySize)
        {
            // 使用 KeyDerivation 生成派生密钥
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256, // 使用 HMACSHA256
                iterationCount: 100000, // 迭代次数
                numBytesRequested: keySize); // 请求的字节数
        }
    }
}
