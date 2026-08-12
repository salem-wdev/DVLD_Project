using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Shared
{
    public class clsUtil
    {
        public static string GenerateGUID()
        {
            return Guid.NewGuid().ToString();
        }
        public static string GenerateGUID(string format)
        {
            return Guid.NewGuid().ToString(format);
        }

        /// <summary>
        /// Computes a SHA-256 hash for the given input string using the standard BitConverter method.
        /// </summary>
        /// <param name="input">The plain text to hash.</param>
        /// <returns>The computed 64-character hexadecimal hash string.</returns>
        public static string ComputeHash(string input)
        {
            // Create an instance of the SHA256 algorithm
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash byte array from the UTF8 bytes of the input string
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert byte array to a hyphen-separated string, remove hyphens, and make it lowercase
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public static string Encrypt(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Set the key and IV for AES encryption
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];


                // Create an encryptor
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);


                // Encrypt the data
                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }


                    // Return the encrypted data as a Base64-encoded string
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Set the key and IV for AES decryption
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];


                // Create a decryptor
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);


                // Decrypt the data
                using (var msDecrypt = new System.IO.MemoryStream(Convert.FromBase64String(cipherText)))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                {
                    // Read the decrypted data from the StreamReader
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
