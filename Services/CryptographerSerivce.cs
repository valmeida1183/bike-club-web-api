using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BikeClub.Services
{
    public static class CryptographerSerivce
    {
        private const string SALT = "643b8ec5cdb641ff9cb149fe5e6a0b29";        

        // Fonte: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-3.1
        public static string Hash(string value)
        {          
            
            byte[] decodedSalt = Convert.FromBase64String(SALT);

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(value, decodedSalt, KeyDerivationPrf.HMACSHA1, 10000, 258/8));
        }

        public static bool CompareHash(string value, string hash)
        {            
           byte[] decodedSalt = Convert.FromBase64String(SALT);
           var hashedValue = Convert.ToBase64String(KeyDerivation.Pbkdf2(value, decodedSalt, KeyDerivationPrf.HMACSHA1, 10000, 258/8));

           return string.Equals(hashedValue, hash);
        }        
    }
}