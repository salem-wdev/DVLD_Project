using System;
using System;
using System.Data;
using System.Data.SqlClient;
using DVLD_Infrastructure.Storage;
using System.Configuration;
using System.Threading.Tasks;
namespace DVLD_DataAccess
{
    public class clsApplicationData
    {
        public static async Task<(bool IsFound, int ApplicantPersonID, DateTime ApplicationDate, int ApplicationTypeID, byte ApplicationStatus, DateTime LastStatusDate, decimal PaidFees, int CreatedByUserID)> GetApplicationInfoByApplicationIDAsync(int ApplicationID)
        {
            bool IsFound = false;
            int ApplicantPersonID = 0;
            DateTime ApplicationDate = default(DateTime);
            int ApplicationTypeID = 0;
            byte ApplicationStatus = 0;
            DateTime LastStatusDate = default(DateTime);
            decimal PaidFees = 0;
            int CreatedByUserID = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = "SELECT [ApplicantPersonID]" +
                                    ",[ApplicationDate]" +
                                    ",[ApplicationTypeID]" +
                                    ",[ApplicationStatus]" +
                                    ",[LastStatusDate]" +
                                    ",[PaidFees]" +
                                    ",[CreatedByUserID]" +
                                   " FROM [Applications]" +
                                   " WHERE [ApplicationID] = @ApplicationID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                ApplicantPersonID = Convert.ToInt32(reader["ApplicantPersonID"]);
                                ApplicationDate = (DateTime)reader["ApplicationDate"];
                                ApplicationTypeID = Convert.ToInt32(reader["ApplicationTypeID"]);
                                ApplicationStatus = Convert.ToByte(reader["ApplicationStatus"]);
                                LastStatusDate = (DateTime)reader["LastStatusDate"];
                                PaidFees = Convert.ToDecimal(reader["PaidFees"]);
                                CreatedByUserID = Convert.ToInt32(reader["CreatedByUserID"]);

                                IsFound = true;
                            }
                            else
                            {
                                IsFound = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (ex) as needed

            }
            return (IsFound, ApplicantPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID);
        }

        public static async Task<(bool IsSucceeded, int ApplicationID, DateTime ApplicationDate, byte ApplicationStatus, DateTime LastStatusDate)> AddNewApplicationAsync(int ApplicantPersonID,
            int ApplicationTypeID,
            decimal PaidFees, int CreatedByUserID)
        {

            int ApplicationID = -1;
            DateTime ApplicationDate = default(DateTime);
            byte ApplicationStatus = 0;
            DateTime LastStatusDate = default(DateTime);

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {


                    string Query = "INSERT INTO [Applications] " +
                        "([ApplicantPersonID], [ApplicationDate], [ApplicationTypeID], [ApplicationStatus], [LastStatusDate], [PaidFees], [CreatedByUserID]) " +
                        "OUTPUT INSERTED.ApplicationID, INSERTED.ApplicationDate, INSERTED.LastStatusDate, INSERTED.ApplicationStatus " +
                        "VALUES " +
                        "(@ApplicantPersonID, GETDATE(), @ApplicationTypeID, 1, GETDATE(), @PaidFees, @CreatedByUserID);";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {

                        command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                        command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                        command.Parameters.AddWithValue("@PaidFees", PaidFees);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);


                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                ApplicationID = Convert.ToInt32(reader["ApplicationID"]);
                                ApplicationDate = (DateTime)reader["ApplicationDate"];
                                LastStatusDate = (DateTime)reader["LastStatusDate"];
                                ApplicationStatus = Convert.ToByte(reader["ApplicationStatus"]);
                            }
                            else
                            {
                                ApplicationID = -1;
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {

            }

            return (ApplicationID > 0, ApplicationID, ApplicationDate, ApplicationStatus, LastStatusDate);

        }

        public static async Task<bool> UpdateApplicationAsync(int ApplicationID,
            decimal PaidFees, int CreatedByUserID)
        {
            int NumberOfEffectedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {

                    string Query = "UPDATE [Applications] " +
                        "SET[PaidFees] = @PaidFees, " +
                        "[CreatedByUserID] = @CreatedByUserID " +
                        "WHERE[ApplicationID] = @ApplicationID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {

                        command.Parameters.AddWithValue("@PaidFees", PaidFees);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);



                        await connection.OpenAsync().ConfigureAwait(false);
                        NumberOfEffectedRows = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
               
            }
            catch (Exception ex)
            {

            }

            return NumberOfEffectedRows > 0;

        }


        public static async Task<DataTable> GetApplicationsPersonListAsync(int ApplicantPersonID)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ApplicationID", typeof(int));
            dt.Columns.Add("ApplicationDate", typeof(DateTime));
            dt.Columns.Add("ApplicationTypeID", typeof(int));
            dt.Columns.Add("ApplicationStatus", typeof(byte));
            dt.Columns.Add("LastStatusDate", typeof(DateTime));
            dt.Columns.Add("PaidFees", typeof(decimal));
            dt.Columns.Add("CreatedByUserID", typeof(int));

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {

                    string Query = "SELECT [ApplicationID]" + ",[ApplicationDate]" +
                                    ",[ApplicationTypeID]" +
                                    ",[ApplicationStatus]" +
                                    ",[LastStatusDate]" +
                                    ",[PaidFees]" +
                                    ",[CreatedByUserID]" +
                                   " FROM [Applications]" +
                                   " WHERE [ApplicantPersonID] = @ApplicantPersonID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                dt.Rows.Add(reader.GetInt32(0), reader.GetDateTime(1), reader.GetInt32(2), reader.GetByte(3),
                                    reader.GetDateTime(4), reader.GetDecimal(5), reader.GetInt32(6));
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

        public static async Task<DataTable> GetApplicationsCreatedByUserListAsync(int CreatedByUserID)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ApplicationID", typeof(int));
            dt.Columns.Add("ApplicantPersonID", typeof(int));
            dt.Columns.Add("ApplicationDate", typeof(DateTime));
            dt.Columns.Add("ApplicationTypeID", typeof(int));
            dt.Columns.Add("ApplicationStatus", typeof(byte));
            dt.Columns.Add("LastStatusDate", typeof(DateTime));
            dt.Columns.Add("PaidFees", typeof(decimal));

            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {

                    string Query = "SELECT [ApplicationID]" +
                                    ",[ApplicantPersonID]" +
                                    ",[ApplicationDate]" +
                                    ",[ApplicationTypeID]" +
                                    ",[ApplicationStatus]" +
                                    ",[LastStatusDate]" +
                                    ",[PaidFees]" +
                                   " FROM [Applications]" +
                                   " WHERE [CreatedByUserID] = @CreatedByUserID";


                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);


                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                dt.Rows.Add(reader.GetInt32(0), reader.GetInt32(1), reader.GetDateTime(2), reader.GetInt32(3), reader.GetByte(4),
                                    reader.GetDateTime(5), reader.GetDecimal(6));
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

        public static async Task<bool> IsApplicationExistAsync(int ApplicationID)
        {
            bool IsFound = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {

                    string Query = "SELECT TOP 1 1 FROM Applications " +
                                     "WHERE ApplicationID = @ApplicationID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);


                        await connection.OpenAsync().ConfigureAwait(false);
                        object Scalar = await command.ExecuteScalarAsync().ConfigureAwait(false);
                        IsFound = (Scalar != null); 
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return IsFound;
        }

        public static async Task<bool> DoesPersonHaveActiveApplicationAsync(int PersonID, int ApplicationTypeID)
        {

            //incase the ActiveApplication ID !=-1 return true.
            return (await GetActiveApplicationIDAsync(PersonID, ApplicationTypeID).ConfigureAwait(false) != -1);
        }

        public static async Task<int> GetActiveApplicationIDAsync(int PersonID, int ApplicationTypeID)
        {
            int ActiveApplicationID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {

                    string query = "SELECT TOP 1 ActiveApplicationID=ApplicationID FROM Applications WHERE ApplicantPersonID = @ApplicantPersonID and ApplicationTypeID=@ApplicationTypeID and ApplicationStatus=1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);
                        command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);


                        await connection.OpenAsync().ConfigureAwait(false);
                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);


                        if (result != null && int.TryParse(result.ToString(), out int AppID))
                        {
                            ActiveApplicationID = AppID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return ActiveApplicationID;
        }

        public static async Task<int> GetActiveApplicationIDForLicenseClassAsync(int PersonID, int ApplicationTypeID, int LicenseClassID)
        {
            int ActiveApplicationID = -1;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {

                    string query = @"SELECT TOP 1 ActiveApplicationID=Applications.ApplicationID  
                            From
                            Applications INNER JOIN
                            LocalDrivingLicenseApplications ON Applications.ApplicationID = LocalDrivingLicenseApplications.ApplicationID
                            WHERE ApplicantPersonID = @ApplicantPersonID 
                            and ApplicationTypeID=@ApplicationTypeID 
							and LocalDrivingLicenseApplications.LicenseClassID = @LicenseClassID
                            and ApplicationStatus=1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);
                        command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                        command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                        await connection.OpenAsync().ConfigureAwait(false);
                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);


                        if (result != null && int.TryParse(result.ToString(), out int AppID))
                        {
                            ActiveApplicationID = AppID;
                        } 
                    }
                    
                }

            }
            catch (Exception ex)
            {

            }
            return ActiveApplicationID;
        }

        public static async Task<bool> DeleteApplicationAsync(int ApplicationID)
        {
            int NumberOfEffectedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = "DELETE FROM Applications " +
                        "WHERE ApplicationID = @ApplicationID";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);


                        await connection.OpenAsync().ConfigureAwait(false);
                        NumberOfEffectedRows = await Command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return NumberOfEffectedRows > 0;
        }

        public static async Task<DataTable> GetAllApplicationsAsync()
        {
            DataTable Table = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = "select * from ApplicationsList_View order by ApplicationDate desc";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await Command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            Table.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Table;
        }

        public static async Task<bool> UpdateStatusAsync(int ApplicationID, short NewStatus, DateTime LastStatusDate)
        {

            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {

                    string query = @"Update  Applications  
                            set 
                                ApplicationStatus = @NewStatus, 
                                LastStatusDate = @LastStatusDate
                            where ApplicationID=@ApplicationID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        command.Parameters.AddWithValue("@NewStatus", NewStatus);
                        command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);



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

        public static async Task<bool> CanApplicationBeEditedAsync(int ApplicationID)
        {
            bool Result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = "SELECT TOP (1) 1 " +
                                "FROM Applications " +
                                "WHERE ApplicationID = @ApplicationID " +
                                "AND ApplicationStatus NOT IN(2, 3); ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                        await connection.OpenAsync().ConfigureAwait(false);
                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);

                        Result = result != null; 
                    }

                }
               
            }
            catch (Exception ex)
            {
            }

            return Result;

        }

        public static async Task<int> GetApplicationTypeIDAsync(int ApplicationID)
        {
            int ApplicationTypeID = -1;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {

                    string query = "SELECT TOP (1) ApplicationTypeID " +
                        "FROM " +
                        "Applications WHERE ApplicationID = @ApplicationID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                        await connection.OpenAsync().ConfigureAwait(false);
                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);


                        if (result != null && int.TryParse(result.ToString(), out int AppTypeID))
                        {
                            ApplicationTypeID = AppTypeID;
                        } 
                    }

                }
            }
            catch (Exception ex)
            {
            }

            return ApplicationTypeID;
        }

        public static async Task<int> GetApplicationStatusAsync(int ApplicationID)
        {
            int ActiveApplicationID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {

                    string query = @"SELECT [ApplicationStatus]
                             FROM [Applications]
                             WHERE [ApplicationID] = @ApplicationID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);


                        await connection.OpenAsync().ConfigureAwait(false);
                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);


                        if (result != null && int.TryParse(result.ToString(), out int AppID))
                        {
                            ActiveApplicationID = AppID;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
            }
            return ActiveApplicationID;
        }


    }
}
