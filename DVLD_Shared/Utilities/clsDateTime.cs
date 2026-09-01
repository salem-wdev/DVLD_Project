using System;

namespace DVLD_Shared.Utilities
{
    /// <summary>
    /// Provides centralized, high-performance, in-memory date and time utilities.
    /// Eliminates network and database roundtrips by serving time calculations directly from memory.
    /// </summary>
    public static class clsDateTime
    {
        /// <summary>
        /// Gets the current system date and time expressed as Coordinated Universal Time (UTC).
        /// Recommended for database storage, auditing, logging, and cross-timezone business logic.
        /// </summary>
        public static DateTime UtcNow => DateTime.UtcNow;

        /// <summary>
        /// Retrieves the current local date and time for Arab Standard Time (UTC+03:00).
        /// Ensures consistent local time calculation across all application components regardless of the host machine settings.
        /// </summary>
        /// <returns>
        /// A <see cref="DateTime"/> representing the current time adjusted to UTC+3.
        /// </returns>
        public static DateTime GetCurrentDateTime()
        {
            return GetDateTime(3); // Arab Standard Time (UTC+3)
        }

        /// <summary>
        /// Retrieves the current date and time adjusted by a specified UTC offset in hours.
        /// Performs in-memory arithmetic on the UTC clock without relying on operating system timezone registries.
        /// </summary>
        /// <param name="utcOffsetHours">
        /// The offset in hours to apply to UTC (e.g., 3 for UTC+3, -5 for UTC-5, 3.5 for UTC+3:30).
        /// </param>
        /// <returns>
        /// A <see cref="DateTime"/> calculated by adding the specified offset hours to <see cref="UtcNow"/>.
        /// </returns>
        public static DateTime GetDateTime(double utcOffsetHours)
        {
            return DateTime.UtcNow.AddHours(utcOffsetHours);
        }
    }
}