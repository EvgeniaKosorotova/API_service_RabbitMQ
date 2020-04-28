using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;

namespace QueueMessageSender.Logic
{
    public class HashGenerator
    {
        public string GetHash(string str)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: str,
                salt: new byte[16],
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }
    }
}
