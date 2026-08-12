using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Shared.Utilities
{
    class clsCryptoHelper
    {
        /// <summary>
        /// Computes a SHA-256 hash for the given input string using the standard BitConverter method.
        /// </summary>
        /// <param name="input">The plain text to hash.</param>
        /// <returns>The computed 64-character hexadecimal hash string.</returns>
        public static string ComputeHash(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");
            // Create an instance of the SHA256 algorithm
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash byte array from the UTF8 bytes of the input string
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert byte array to a hyphen-separated string, remove hyphens, and make it lowercase
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Encrypts a plain text string using the AES algorithm and a specified key.
        /// </summary>
        /// <param name="plainText">The plain text string to encrypt.</param>
        /// <param name="key">The secret key used for encryption (must be 16, 24, or 32 characters long).</param>
        /// <returns>A Base64-encoded string representing the encrypted cipher text.</returns>
        public static string Encrypt(string plainText, string key)
        {
            if (plainText == null)
                throw new ArgumentNullException(nameof(plainText), "plain Text cannot be null.");

            if (key == null)
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");

            using (Aes aesAlg = Aes.Create())
            {
                // Convert the key string into a UTF-8 byte array
                aesAlg.Key = Encoding.UTF8.GetBytes(key);

                // Initialize a zero-filled Initialization Vector (IV) matching the AES block size (16 bytes)
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                // Create the encryptor transform object with the key and IV
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Perform encryption using nested memory and crypto streams
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // Write plain text to the stream to transform it into cipher bytes
                        swEncrypt.Write(plainText);
                    }

                    // Return the resulting cipher bytes as a Base64-encoded string
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        /// <summary>
        /// Decrypts a Base64-encoded cipher text string back to plain text using the AES algorithm and key.
        /// </summary>
        /// <param name="cipherText">The Base64-encoded encrypted string to decrypt.</param>
        /// <param name="key">The secret key used during encryption.</param>
        /// <returns>The original decrypted plain text string.</returns>
        public static string Decrypt(string cipherText, string key)
        {
            if (cipherText == null)
                throw new ArgumentNullException(nameof(cipherText), "cipher Text cannot be null.");

            if (key == null)
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");

            using (Aes aesAlg = Aes.Create())
            {
                // Convert the key string into a UTF-8 byte array
                aesAlg.Key = Encoding.UTF8.GetBytes(key);

                // Initialize the same zero-filled Initialization Vector (IV) used for encryption
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                // Create the decryptor transform object with the key and IV
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Convert Base64 cipher text back to raw bytes and prepare decryption streams
                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    // Read all decrypted bytes from the stream and return as plain text
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
