using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLib
{
    public class CryptoSecurity
    {
        private static string MasterEncrypt(string toEncrypt)
        {
            byte[] keyArray;
            byte[] iv;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string MasterKey = "ALVIN5@NT05";
            string MasterIV = "@LVIN123";

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(MasterKey));
            iv = UTF8Encoding.UTF8.GetBytes(MasterIV);
            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                IV = iv,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Encrypt(string toEncrypt)
        {
            byte[] keyArray;
            byte[] iv;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.GenerateKey();
            keyArray = tdes.Key;
            tdes.GenerateIV();
            iv = tdes.IV;
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            string MasterPlainText = Convert.ToBase64String(keyArray, 0, keyArray.Length) + Convert.ToBase64String(resultArray, 0, resultArray.Length) + Convert.ToBase64String(iv, 0, iv.Length);
            return MasterEncrypt(MasterPlainText);
        }

        private static string MasterDecrypt(string cipherString)
        {
            byte[] keyArray;
            byte[] iv;
            byte[] toDecryptArray = Convert.FromBase64String(cipherString);

            string MasterKey = "ALVIN5@NT05";
            string MasterIV = "@LVIN123";

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(MasterKey));
            iv = UTF8Encoding.UTF8.GetBytes(MasterIV);
            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                IV = iv,

                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
            tdes.Clear();


            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static string Decrypt(string cipherString)
        {
            byte[] key;
            byte[] iv;
            byte[] toDecryptArray;

            string toDecryptString = MasterDecrypt(cipherString);

            string descryptString = toDecryptString.Substring(32, toDecryptString.Length - 44);
            string keyString = toDecryptString.Substring(0, 32);
            string ivString = toDecryptString.Substring(toDecryptString.Length - 12, 12);

            key = Convert.FromBase64String(keyString);
            iv = Convert.FromBase64String(ivString);
            toDecryptArray = Convert.FromBase64String(descryptString);



            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = key,
                IV = iv,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
            tdes.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
