using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DVLD_Infrastructure.Storage;

namespace DVLD_DataAccess
{
    public class clsDriverData
    {
        public static async Task<(bool IsFound, int PersonID, int CreatedByUserID, DateTime CreatedDate)> GetDriverInfoByDriverIDAsync(int DriverID)
        {
            int PersonID = -1, CreatedByUserID = -1;
            DateTime CreatedDate = DateTime.Now;

            bool isFound = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = "SELECT PersonID, CreatedByUserID, CreatedDate FROM Drivers WHERE DriverID = @DriverID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;


                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {

                                // The record was found
                                isFound = true;

                                PersonID = reader.GetInt32(0);
                                CreatedByUserID = reader.GetInt32(1);
                                CreatedDate = reader.GetDateTime(2);


                            }
                            else
                            {
                                // The record was not found
                                isFound = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to get driver info for DriverID: {DriverID}");
            }
            return (isFound, PersonID, CreatedByUserID, CreatedDate);
        }

        public static async Task<(bool IsFound, int DriverID, int CreatedByUserID, DateTime CreatedDate)> GetDriverInfoByPersonIDAsync(int? PersonID)
        {
            bool isFound = false;
            int DriverID = -1;
            int CreatedByUserID = -1;
            DateTime CreatedDate = DateTime.MinValue;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = "SELECT DriverID, CreatedByUserID, CreatedDate FROM Drivers WHERE PersonID = @PersonID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;


                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {

                                // The record was found
                                isFound = true;

                                DriverID = reader.GetInt32(0);
                                CreatedByUserID = reader.GetInt32(1);
                                CreatedDate = reader.GetDateTime(2);

                            }
                            else
                            {
                                // The record was not found
                                isFound = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to get driver info for PersonID: {PersonID}");
            }
            return (isFound, DriverID, CreatedByUserID, CreatedDate);
        }

        public static async Task<DataTable> GetAllDriversAsync()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("DriverID", typeof(int));
            dt.Columns.Add("PersonID", typeof(int));
            dt.Columns.Add("NationalNo", typeof(string));
            dt.Columns.Add("FullName", typeof(string));
            dt.Columns.Add("CreatedDate", typeof(DateTime));
            dt.Columns.Add("NumberOfActiveLicenses", typeof(int));

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = "SELECT DriverID, PersonID, NationalNo, FullName, CreatedDate, NumberOfActiveLicenses FROM Drivers_View order by FullName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                           while (await reader.ReadAsync().ConfigureAwait(false))
                           {
                               dt.Rows.Add(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3), reader.GetDateTime(4), reader.GetInt32(5));
                           }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Failed to get all drivers");
            }
            return dt;
        }

        public static async Task<int> AddNewDriverAsync(int? PersonID, int CreatedByUserID)
        {
            int DriverID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"INSERT INTO Drivers (PersonID, CreatedByUserID, CreatedDate)
                                            VALUES (@PersonID, @CreatedByUserID, GETDATE());                                
                                            SELECT SCOPE_IDENTITY();";


                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID.HasValue ? PersonID.Value : (object)DBNull.Value;
                        command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = CreatedByUserID;

                        await connection.OpenAsync().ConfigureAwait(false);

                       using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                       {
                           if (await reader.ReadAsync().ConfigureAwait(false))
                           {
                               DriverID = reader.GetInt32(0);
                           }
                       }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to add new driver for PersonID: {PersonID}, CreatedByUserID: {CreatedByUserID}");
            }

            return DriverID;

        }

        public static async Task<bool> UpdateDriverAsync(int DriverID, int? PersonID, int CreatedByUserID)
        {

            int rowsAffected = 0;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {//we dont update the createddate for the driver.
                    string query = @"Update  Drivers  
                            set PersonID = @PersonID,
                                CreatedByUserID = @CreatedByUserID
                                where DriverID = @DriverID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID.HasValue ? PersonID.Value : (object)DBNull.Value;
                        command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = CreatedByUserID;


                        await connection.OpenAsync().ConfigureAwait(false);
                        rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to update driver with DriverID: {DriverID}, PersonID: {PersonID}, CreatedByUserID: {CreatedByUserID}");
            }

            return (rowsAffected > 0);
        }

    }
}
