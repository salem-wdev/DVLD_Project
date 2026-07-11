using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business.Extensions
{
    /// <summary>
    /// Provides extension methods for array types to streamline common validation logic.
    /// </summary>
    public static class StringArrayExtensions
    {
        /// <summary>
        /// Determines whether any element within the provided string array is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="values">The array of strings to evaluate.</param>
        /// <returns>True if at least one element is null or white space; otherwise, false.</returns>
        public static bool AreAnyEmpty(this string[] values)
        {
            if (values == null || values.Length == 0)
            {
                return false;
            }

            return values.Any(string.IsNullOrWhiteSpace);
        }
    }
}
