using DVLD_DataAccess;
using System;

namespace DVLD_Business
{
    /// <summary>
    /// Provides centralized management for global application settings, 
    /// configuration data, and shared utility functions across the business layer.
    /// </summary>
    public static class clsBusinessSettings
    {
        /// <summary>
        /// Retrieves the current date and time directly from the database server.
        /// Used to ensure time synchronization across all critical business operations 
        /// and prevent client-side tampering.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> representing the current server date and time.</returns>
        public static DateTime GetServerDateTime()
        {
            return clsUtilData.GetServerDate();
        }
    }
}
