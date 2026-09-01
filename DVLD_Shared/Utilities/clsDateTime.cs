using System;

namespace DVLD_Shared.Utilities
{
    /// <summary>
    /// Provides centralized, in-memory date and time utilities across the application.
    /// Eliminates network overhead by avoiding direct database clock queries.
    /// </summary>
    public class clsDateTime
    {
        /// <summary>
        /// Gets the current system date and time expressed as Coordinated Universal Time (UTC).
        /// Recommended for all database persistence, auditing, and business calculations.
        /// </summary>
        public static DateTime UtcNow => DateTime.UtcNow;

        /// <summary>
        /// Retrieves the current date and time converted to the Arab Standard Time zone (UTC+03:00).
        /// Ensures consistent local time calculation regardless of the host server's local time zone configuration.
        /// </summary>
        /// <returns>
        /// A <see cref="DateTime"/> value representing the current local time for Arab Standard Time (UTC+3).
        /// </returns>
        public static DateTime GetServerDate()
        {
            TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localZone);
        }
    }
}
