using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DVLD_Infrastructure.Storage;

namespace DVLD_DataAccess
{
    public class clsInternationalLicenseData
    {

        public static async Task<(bool IsFound, int ApplicationID, int DriverID, int IssuedUsingLocalLicenseID, DateTime IssueDate, DateTime ExpirationDate, bool IsActive, int CreatedByUserID)> GetInternationalLicenseInfoByIDAsync(int InternationalLicenseID)
        {
            int ApplicationID = -1;
            int DriverID = -1;
            int IssuedUsingLocalLicenseID = -1;
            DateTime IssueDate = DateTime.MinValue;
            DateTime ExpirationDate = DateTime.MinValue;
            bool IsActive = false;
            int CreatedByUserID = -1;
            bool isFound = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = "SELECT * FROM InternationalLicenses WHERE InternationalLicenseID = @InternationalLicenseID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);


                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {

                                // The record was found
                                isFound = true;
                                ApplicationID = (int)reader["ApplicationID"];
                                DriverID = (int)reader["DriverID"];
                                IssuedUsingLocalLicenseID = (int)reader["IssuedUsingLocalLicenseID"];
                                IssueDate = (DateTime)reader["IssueDate"];
                                ExpirationDate = (DateTime)reader["ExpirationDate"];


                                IsActive = (bool)reader["IsActive"];
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
            }

            return (isFound, ApplicationID, DriverID, IssuedUsingLocalLicenseID, IssueDate, ExpirationDate, IsActive, CreatedByUserID);
        }

        public static async Task<DataTable> GetAllInternationalLicensesAsync()
        {

            DataTable dt = new DataTable();

            dt.Columns.Add("InternationalLicenseID", typeof(int));
            dt.Columns.Add("ApplicationID", typeof(int));
            dt.Columns.Add("DriverID", typeof(int));
            dt.Columns.Add("IssuedUsingLocalLicenseID", typeof(int));
            dt.Columns.Add("IssueDate", typeof(DateTime));
            dt.Columns.Add("ExpirationDate", typeof(DateTime));
            dt.Columns.Add("IsActive", typeof(bool));

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT    InternationalLicenseID, ApplicationID,DriverID,
		                                          IssuedUsingLocalLicenseID , IssueDate, 
                                                  ExpirationDate, IsActive
		                              from InternationalLicenses 
                                          order by IsActive, ExpirationDate desc";

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
                                    reader.GetInt32(2),
                                    reader.GetInt32(3),
                                    reader.GetDateTime(4),
                                    reader.GetDateTime(5),
                                    reader.GetBoolean(6)
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return dt;

        }

        public static async Task<DataTable> GetDriverInternationalLicensesAsync(int DriverID)
        {

            DataTable dt = new DataTable();

            dt.Columns.Add("InternationalLicenseID", typeof(int));
            dt.Columns.Add("ApplicationID", typeof(int));
            dt.Columns.Add("IssuedUsingLocalLicenseID", typeof(int));
            dt.Columns.Add("IssueDate", typeof(DateTime));
            dt.Columns.Add("ExpirationDate", typeof(DateTime));
            dt.Columns.Add("IsActive", typeof(bool));

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT    InternationalLicenseID, ApplicationID,
		                                       IssuedUsingLocalLicenseID , IssueDate, 
                                               ExpirationDate, IsActive
		                                       from InternationalLicenses where DriverID=@DriverID
                                               order by ExpirationDate desc";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DriverID", DriverID);

                        await connection.OpenAsync().ConfigureAwait(false);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                dt.Rows.Add(
                                    reader.GetInt32(0),
                                    reader.GetInt32(1),
                                    reader.GetInt32(2),
                                    reader.GetDateTime(3),
                                    reader.GetDateTime(4),
                                    reader.GetBoolean(5)
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return dt;

        }


        public static async Task<int> AddNewInternationalLicenseAsync(int ApplicationID,
             int DriverID, int IssuedUsingLocalLicenseID,
             DateTime IssueDate, DateTime ExpirationDate, bool IsActive, int CreatedByUserID)
        {
            int InternationalLicenseID = -1;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"
                               Update InternationalLicenses 
                               set IsActive=0
                               where DriverID=@DriverID;

                             INSERT INTO InternationalLicenses
                               (
                                ApplicationID,
                                DriverID,
                                IssuedUsingLocalLicenseID,
                                IssueDate,
                                ExpirationDate,
                                IsActive,
                                CreatedByUserID)
                         VALUES
                               (@ApplicationID,
                                @DriverID,
                                @IssuedUsingLocalLicenseID,
                                @IssueDate,
                                @ExpirationDate,
                                @IsActive,
                                @CreatedByUserID);
                            SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        command.Parameters.AddWithValue("@DriverID", DriverID);
                        command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
                        command.Parameters.AddWithValue("@IssueDate", IssueDate);
                        command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);

                        command.Parameters.AddWithValue("@IsActive", IsActive);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        await connection.OpenAsync().ConfigureAwait(false);

                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            InternationalLicenseID = insertedID;
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }


            return InternationalLicenseID;

        }

        public static async Task<bool> UpdateInternationalLicenseAsync(
              int InternationalLicenseID, int ApplicationID,
             int DriverID, int IssuedUsingLocalLicenseID,
             DateTime IssueDate, DateTime ExpirationDate, bool IsActive, int CreatedByUserID)
        {

            int rowsAffected = 0; try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"UPDATE InternationalLicenses
                                            SET 
                                               ApplicationID=@ApplicationID,
                                               DriverID = @DriverID,
                                               IssuedUsingLocalLicenseID = @IssuedUsingLocalLicenseID,
                                               IssueDate = @IssueDate,
                                               ExpirationDate = @ExpirationDate,
                                               IsActive = @IsActive,
                                               CreatedByUserID = @CreatedByUserID
                                            WHERE InternationalLicenseID=@InternationalLicenseID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        command.Parameters.AddWithValue("@DriverID", DriverID);
                        command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
                        command.Parameters.AddWithValue("@IssueDate", IssueDate);
                        command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);

                        command.Parameters.AddWithValue("@IsActive", IsActive);
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

        public static async Task<int> GetActiveInternationalLicenseIDByDriverIDAsync(int DriverID)
        {
            int InternationalLicenseID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT TOP (1) InternationalLicenseID
                             FROM [InternationalLicenses]
                             WHERE DriverID = @DriverID AND IsActive = 1 AND GETDATE() BETWEEN IssueDate AND ExpirationDate
                             ORDER BY InternationalLicenseID DESC;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DriverID", DriverID);

                        await connection.OpenAsync().ConfigureAwait(false);

                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            InternationalLicenseID = insertedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }


            return InternationalLicenseID;
        }

        public static async Task<bool> DeactvateInternationalLicensesforExpiredLocalLicensesAsync()
        {

            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"  UPDATE [InternationalLicenses]
                                   SET [IsActive] = 0
                                 WHERE ExpirationDate < GETDATE() AND IsActive = 1;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
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
    }
}
