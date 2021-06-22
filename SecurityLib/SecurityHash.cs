using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLib
{
    public class SecurityHash
    {
        public static string Encrypt(string plaintext, string salt)
        {
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(plaintext);
            byte[] bytesalt = UTF8Encoding.UTF8.GetBytes(salt);

            var pbkdf2 = new Rfc2898DeriveBytes(toEncryptArray, bytesalt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            return Convert.ToBase64String(hash);
        }

        public static string CreateSalt()
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[20];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }
    }
}
