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
    }
}
