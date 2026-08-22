using DVLD_DataAccess;
using DVLD_Infrastructure.Storage;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsDetainedLicenseData
    {
        public static async Task<(bool IsFound, int LicenseID, DateTime DetainDate, float FineFees, int CreatedByUserID, bool IsReleased, DateTime ReleaseDate, int ReleasedByUserID, int ReleaseApplicationID)> GetDetainedLicenseInfoByIDAsync(int DetainID)
        {
            int LicenseID = -1;
            DateTime DetainDate = DateTime.MinValue;
            float FineFees = 0;
            int CreatedByUserID = -1;
            bool IsReleased = false;
            DateTime ReleaseDate = DateTime.MaxValue;
            int ReleasedByUserID = -1;
            int ReleaseApplicationID = -1;
            bool isFound = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = "SELECT * FROM DetainedLicenses WHERE DetainID = @DetainID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DetainID", DetainID);


                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {

                                // The record was found
                                isFound = true;

                                LicenseID = (int)reader["LicenseID"];
                                DetainDate = (DateTime)reader["DetainDate"];
                                FineFees = Convert.ToSingle(reader["FineFees"]);
                                CreatedByUserID = (int)reader["CreatedByUserID"];

                                IsReleased = (bool)reader["IsReleased"];

                                if (reader["ReleaseDate"] == DBNull.Value)

                                    ReleaseDate = DateTime.MaxValue;
                                else
                                    ReleaseDate = (DateTime)reader["ReleaseDate"];


                                if (reader["ReleasedByUserID"] == DBNull.Value)

                                    ReleasedByUserID = -1;
                                else
                                    ReleasedByUserID = (int)reader["ReleasedByUserID"];

                                if (reader["ReleaseApplicationID"] == DBNull.Value)

                                    ReleaseApplicationID = -1;
                                else
                                    ReleaseApplicationID = (int)reader["ReleaseApplicationID"];

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                
            }


            return (isFound, LicenseID, DetainDate, FineFees, CreatedByUserID, IsReleased, ReleaseDate, ReleasedByUserID, ReleaseApplicationID);
        }


        public static async Task<(bool IsFound, int DetainID, DateTime DetainDate, float FineFees, int CreatedByUserID, bool IsReleased, DateTime ReleaseDate, int ReleasedByUserID, int ReleaseApplicationID)> GetDetainedLicenseInfoByLicenseIDAsync(int LicenseID)
        {
            int DetainID = -1;
            DateTime DetainDate = DateTime.MinValue;
            float FineFees = 0;
            int CreatedByUserID = -1;
            bool IsReleased = false;
            DateTime ReleaseDate = DateTime.MaxValue;
            int ReleasedByUserID = -1;
            int ReleaseApplicationID = -1;
            bool isFound = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = "SELECT top 1 * FROM DetainedLicenses WHERE LicenseID = @LicenseID order by DetainID desc";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LicenseID", LicenseID);


                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {

                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {

                                // The record was found
                                isFound = true;

                                DetainID = (int)reader["DetainID"];
                                DetainDate = (DateTime)reader["DetainDate"];
                                FineFees = Convert.ToSingle(reader["FineFees"]);
                                CreatedByUserID = (int)reader["CreatedByUserID"];

                                IsReleased = (bool)reader["IsReleased"];

                                if (reader["ReleaseDate"] == DBNull.Value)

                                    ReleaseDate = DateTime.MaxValue;
                                else
                                    ReleaseDate = (DateTime)reader["ReleaseDate"];


                                if (reader["ReleasedByUserID"] == DBNull.Value)

                                    ReleasedByUserID = -1;
                                else
                                    ReleasedByUserID = (int)reader["ReleasedByUserID"];

                                if (reader["ReleaseApplicationID"] == DBNull.Value)

                                    ReleaseApplicationID = -1;
                                else
                                    ReleaseApplicationID = (int)reader["ReleaseApplicationID"];

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

            }
            return (isFound, DetainID, DetainDate, FineFees, CreatedByUserID, IsReleased, ReleaseDate, ReleasedByUserID, ReleaseApplicationID);
        }

        public static async Task<DataTable> GetAllDetainedLicensesAsync()
        {

            DataTable dt = new DataTable();

            dt.Columns.Add("DetainID", typeof(int));
            dt.Columns.Add("LicenseID", typeof(int));
            dt.Columns.Add("DetainDate", typeof(DateTime));
            dt.Columns.Add("IsReleased", typeof(bool));
            dt.Columns.Add("FineFees", typeof(decimal));
            dt.Columns.Add("ReleaseDate", typeof(DateTime));
            dt.Columns.Add("NationalNo", typeof(string));
            dt.Columns.Add("FullName", typeof(string));
            dt.Columns.Add("ReleaseApplicationID", typeof(int));


            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"select DetainID, LicenseID, DetainDate, IsReleased, FineFees,
                                        ReleaseDate, NationalNo, FullName, ReleaseApplicationID
                                        from detainedLicenses_View order by IsReleased ,DetainID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        await connection.OpenAsync().ConfigureAwait(false);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                dt.Rows.Add(
                                    reader.GetInt32(0),
                                    reader.GetInt32(1),
                                    reader.GetDateTime(2),
                                    reader.GetBoolean(3),
                                    reader.GetDecimal(4),
                                    reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                    reader.GetString(6),
                                    reader.GetString(7),
                                    reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8)
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while retrieving all detained licenses.");
            }

            return dt;

        }

        public static async Task<int> AddNewDetainedLicenseAsync(
            int LicenseID, DateTime DetainDate,
            float FineFees, int CreatedByUserID)
        {
            int DetainID = -1;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"INSERT INTO DetainedLicenses
                               (LicenseID,
                               DetainDate,
                               FineFees,
                               CreatedByUserID,
                               IsReleased
                               )
                            VALUES
                               (@LicenseID,
                               @DetainDate, 
                               @FineFees, 
                               @CreatedByUserID,
                               0
                             );
                            
                            SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LicenseID", LicenseID);
                        command.Parameters.AddWithValue("@DetainDate", DetainDate);
                        command.Parameters.AddWithValue("@FineFees", FineFees);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);


                        await connection.OpenAsync().ConfigureAwait(false);

                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            DetainID = insertedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }


            return DetainID;

        }

        public static async Task<bool> UpdateDetainedLicenseAsync(int DetainID,
            int LicenseID, DateTime DetainDate,
            float FineFees, int CreatedByUserID)
        {

            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"UPDATE DetainedLicenses
                              SET LicenseID = @LicenseID, 
                              DetainDate = @DetainDate, 
                              FineFees = @FineFees,
                              CreatedByUserID = @CreatedByUserID,   
                              WHERE DetainID=@DetainID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DetainedLicenseID", DetainID);
                        command.Parameters.AddWithValue("@LicenseID", LicenseID);
                        command.Parameters.AddWithValue("@DetainDate", DetainDate);
                        command.Parameters.AddWithValue("@FineFees", FineFees);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);



                        await connection.OpenAsync().ConfigureAwait(false);
                        rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return (rowsAffected > 0);
        }


        public static async Task<bool> ReleaseDetainedLicenseAsync(int DetainID,
                 DateTime ReleaseDate, int ReleasedByUserID, int ReleaseApplicationID)
        {

            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"UPDATE DetainedLicenses
                              SET IsReleased = 1, 
                              ReleaseDate = @ReleaseDate,
                              ReleasedByUserID = @ReleasedByUserID,
                              ReleaseApplicationID = @ReleaseApplicationID   
                              WHERE DetainID=@DetainID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DetainID", DetainID);
                        command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);
                        command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);
                        command.Parameters.AddWithValue("@ReleaseDate", ReleaseDate);

                        await connection.OpenAsync().ConfigureAwait(false);
                        rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return (rowsAffected > 0);
        }

        public static async Task<bool> IsLicenseDetainedAsync(int LicenseID)
        {
            bool IsDetained = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"select IsDetained=1 
                            from detainedLicenses 
                            where 
                            LicenseID=@LicenseID 
                            and IsReleased=0;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LicenseID", LicenseID);


                        await connection.OpenAsync().ConfigureAwait(false);

                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);

                        if (result != null)
                        {
                            IsDetained = Convert.ToBoolean(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }


            return IsDetained;
            ;

        }

    }
}
