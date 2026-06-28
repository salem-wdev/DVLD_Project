using System;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public class clsUtilData
    {
        /// <summary>
        /// Connects to the database and retrieves the current date and time from the server.
        /// </summary>
        /// <returns>The current DateTime from the SQL Server, or the local machine time if the connection fails.</returns>
        public static DateTime GetServerDate()
        {
            // Set a default fallback value in case the database connection fails
            DateTime serverDate = DateTime.Now;

            // Simple query to fetch the current time from the server
            string query = "SELECT GETDATE();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    // Using ExecuteScalar because we expect exactly one single value (one cell)
                    object result = command.ExecuteScalar();

                    if (result != null && DateTime.TryParse(result.ToString(), out DateTime date))
                    {
                        serverDate = date;
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Implement error logging here (e.g., Event Viewer or a text file)
                // For now, it will safely fall back to returning the local machine's DateTime.Now
            }

            return serverDate;
        }
    }
}
