using DVLD_Shared;
using DVLD_Infrastructure.Storage;
using System;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DVLD_DataAccess
{
    public class clsLicenseData
    {
        public static async Task<(bool IsFound, int ApplicationID, int DriverID, int LicenseClass, DateTime IssueDate, DateTime ExpirationDate, string Notes, float PaidFees, bool IsActive, byte IssueReason, int CreatedByUserID)> GetLicenseInfoByIDAsync(int LicenseID)
        {
            int ApplicationID = -1;
            int DriverID = -1;
            int LicenseClass = -1;
            DateTime IssueDate = DateTime.MinValue;
            DateTime ExpirationDate = DateTime.MinValue;
            string Notes = string.Empty;
            float PaidFees = 0;
            bool IsActive = false;
            byte IssueReason = 0;
            int CreatedByUserID = -1;
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = "SELECT * FROM Licenses WHERE LicenseID = @LicenseID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.Add("@LicenseID", SqlDbType.Int).Value = LicenseID;


                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {

                                // The record was found
                                isFound = true;
                                ApplicationID = (int)reader["ApplicationID"];
                                DriverID = (int)reader["DriverID"];
                                LicenseClass = (int)reader["LicenseClass"];
                                IssueDate = (DateTime)reader["IssueDate"];
                                ExpirationDate = (DateTime)reader["ExpirationDate"];

                                if (reader["Notes"] == DBNull.Value)
                                    Notes = "";
                                else
                                    Notes = (string)reader["Notes"];

                                PaidFees = Convert.ToSingle(reader["PaidFees"]);
                                IsActive = (bool)reader["IsActive"];
                                IssueReason = (byte)reader["IssueReason"];
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                            }
                        }
                    }


                }

            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve License info for LicenseID: {LicenseID}");

            }

            return (isFound, ApplicationID, DriverID, LicenseClass, IssueDate, ExpirationDate, Notes, PaidFees, IsActive, IssueReason, CreatedByUserID);
        }

        public static async Task<(bool IsFound, int LicenseID, int DriverID, int LicenseClass, DateTime IssueDate, DateTime ExpirationDate, string Notes, float PaidFees, bool IsActive, byte IssueReason, int CreatedByUserID)> GetLicenseInfoByApplicationIDAsync(int ApplicationID)
        {
            int LicenseID = -1;
            int DriverID = -1;
            int LicenseClass = -1;
            DateTime IssueDate = DateTime.MinValue;
            DateTime ExpirationDate = DateTime.MinValue;
            string Notes = string.Empty;
            float PaidFees = 0;
            bool IsActive = false;
            byte IssueReason = 0;
            int CreatedByUserID = -1;
            bool isFound = false;

            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = "SELECT * FROM Licenses WHERE ApplicationID = @ApplicationID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {

                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {

                                // The record was found
                                isFound = true;
                                LicenseID = (int)reader["LicenseID"];
                                DriverID = (int)reader["DriverID"];
                                LicenseClass = (int)reader["LicenseClass"];
                                IssueDate = (DateTime)reader["IssueDate"];
                                ExpirationDate = (DateTime)reader["ExpirationDate"];

                                if (reader["Notes"] == DBNull.Value)
                                    Notes = "";
                                else
                                    Notes = (string)reader["Notes"];

                                PaidFees = Convert.ToSingle(reader["PaidFees"]);
                                IsActive = (bool)reader["IsActive"];
                                IssueReason = (byte)reader["IssueReason"];
                                CreatedByUserID = (int)reader["CreatedByUserID"];


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
                clsLogger.LogException(ex, $"Failed to retrieve License info for ApplicationID: {ApplicationID}");
            }
            return (isFound, LicenseID, DriverID, LicenseClass, IssueDate, ExpirationDate, Notes, PaidFees, IsActive, IssueReason, CreatedByUserID);
        }


        public static async Task<DataTable> GetAllLicensesAsync()
        {

            DataTable dt = new DataTable();

            dt.Columns.Add("LicenseID",typeof(int));
            dt.Columns.Add("ApplicationID", typeof(int));
            dt.Columns.Add("DriverID", typeof(int));
            dt.Columns.Add("LicenseClass", typeof(int));
            dt.Columns.Add("IssueDate", typeof(DateTime));
            dt.Columns.Add("ExpirationDate", typeof(DateTime));
            dt.Columns.Add("Notes", typeof(string));
            dt.Columns.Add("PaidFees", typeof(decimal));
            dt.Columns.Add("IsActive", typeof(bool));
            dt.Columns.Add("IssueReason", typeof(byte));
            dt.Columns.Add("CreatedByUserID", typeof(int));


            try
            {   using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT LicenseID, ApplicationID, DriverID, LicenseClass,
                                     IssueDate, ExpirationDate, Notes, PaidFees, IsActive, IssueReason, CreatedByUserID
                                     FROM Licenses";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                dt.Rows.Add(
                                    reader.GetInt32(0),     // LicenseID
                                    reader.GetInt32(1),     // ApplicationID
                                    reader.GetInt32(2),     // DriverID
                                    reader.GetInt32(3),     // LicenseClass
                                    reader.GetDateTime(4),  // IssueDate
                                    reader.GetDateTime(5),  // ExpirationDate
                                    reader.GetString(6),    // Notes
                                    reader.GetDecimal(7),   // PaidFees
                                    reader.GetBoolean(8),   // IsActive
                                    reader.GetByte(9),      // IssueReason
                                    reader.GetInt32(10)     // CreatedByUserID
                                    );                      
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Failed to retrieve all licenses.");
            }
            return dt;

        }

        public static async Task<DataTable> GetDriverLicensesAsync(int DriverID)
        {

            DataTable dt = new DataTable();

            dt.Columns.Add("LicenseID", typeof(int));
            dt.Columns.Add("ApplicationID", typeof(int));
            dt.Columns.Add("ClassName", typeof(string));
            dt.Columns.Add("IssueDate", typeof(DateTime));
            dt.Columns.Add("ExpirationDate", typeof(DateTime));
            dt.Columns.Add("IsActive", typeof(bool));

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT     
                           Licenses.LicenseID,
                           ApplicationID,
		                   LicenseClasses.ClassName, Licenses.IssueDate, 
		                   Licenses.ExpirationDate, Licenses.IsActive
                           FROM Licenses INNER JOIN
                                LicenseClasses ON Licenses.LicenseClass = LicenseClasses.LicenseClassID
                            where DriverID=@DriverID
                            Order By IsActive Desc, ExpirationDate Desc";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;
                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                dt.Rows.Add(
                                    reader.GetInt32(0),     // LicenseID
                                    reader.GetInt32(1),     // ApplicationID
                                    reader.GetString(3),    // ClassName
                                    reader.GetDateTime(4),  // IssueDate
                                    reader.GetDateTime(5),  // ExpirationDate
                                    reader.GetBoolean(8)    // IsActive
                                    );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve licenses for DriverID: {DriverID}");
            }

            return dt;
        }

        public static async Task<(int LicenseID, DateTime IssueDate, DateTime ExpirationDate)> AddNewLicenseAsync(int ApplicationID, int DriverID, int LicenseClass,
            string Notes, float PaidFees, bool IsActive, byte IssueReason, int CreatedByUserID)
        {
            int LicenseID = -1;
            DateTime IssueDate = DateTime.MinValue;
            DateTime ExpirationDate = DateTime.MinValue;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"DECLARE @CalculatedExpirationDate DATETIME;
                                DECLARE @CurrentDate DATETIME = GETDATE();
                                
                                IF @IssueReason = 3 OR @IssueReason = 4 
                                BEGIN
                                
                                    SELECT TOP 1 @CalculatedExpirationDate = ExpirationDate 
                                    FROM [dbo].[Licenses] 
                                    WHERE DriverID = @DriverID AND LicenseClass = @LicenseClass
                                    ORDER BY LicenseID DESC; 
                                END
                                ELSE 
                                BEGIN
                                
                                    SET @CalculatedExpirationDate = DATEADD(year, 
                                        (SELECT DefaultValidityLength FROM [dbo].[LicenseClasses] WHERE LicenseClassID = @LicenseClass), 
                                        @CurrentDate
                                    );
                                END
                                
                                
                                INSERT INTO [dbo].[Licenses]
                                           ([ApplicationID]
                                           ,[DriverID]
                                           ,[LicenseClass]
                                           ,[IssueDate]
                                           ,[ExpirationDate]
                                           ,[Notes]
                                           ,[PaidFees]
                                           ,[IsActive]
                                           ,[IssueReason]
                                           ,[CreatedByUserID])
                                     VALUES
                                           (@ApplicationID
                                           ,@DriverID
                                           ,@LicenseClass
                                           ,@CurrentDate                
                                           ,@CalculatedExpirationDate 
                                           ,@Notes
                                           ,@PaidFees
                                           ,@IsActive
                                           ,@IssueReason
                                           ,@CreatedByUserID);
                                
                                
                                SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewLicenseID, @CurrentDate AS DB_IssueDate, @CalculatedExpirationDate AS DB_ExpirationDate;";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;
                        command.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;
                        command.Parameters.Add("@LicenseClass", SqlDbType.Int).Value = LicenseClass;

                        if (Notes == "")
                            command.Parameters.Add("@Notes", SqlDbType.NVarChar, 500).Value = DBNull.Value;
                        else
                            command.Parameters.Add("@Notes", SqlDbType.NVarChar, 500).Value = Notes;

                        command.Parameters.Add("@PaidFees", SqlDbType.SmallMoney).Value = PaidFees;
                        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = IsActive;
                        command.Parameters.Add("@IssueReason", SqlDbType.TinyInt).Value = IssueReason;

                        command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = CreatedByUserID;

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                LicenseID = reader.GetInt32(0);
                                IssueDate = reader.GetDateTime(1);
                                ExpirationDate = reader.GetDateTime(2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to add new license for DriverID: {DriverID}, LicenseClass: {LicenseClass}");
            }
            return (LicenseID, IssueDate, ExpirationDate);
        }

        public static async Task<bool> UpdateLicenseAsync(int LicenseID, int ApplicationID, int DriverID, int LicenseClass,
             string Notes, float PaidFees, bool IsActive, byte IssueReason,
             int CreatedByUserID)
        {

            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"UPDATE Licenses
                           SET ApplicationID=@ApplicationID, DriverID = @DriverID,
                              LicenseClass = @LicenseClass,
                              Notes = @Notes,
                              PaidFees = @PaidFees,
                              IsActive = @IsActive,IssueReason=@IssueReason,
                              CreatedByUserID = @CreatedByUserID
                         WHERE LicenseID=@LicenseID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LicenseID", SqlDbType.Int).Value = LicenseID;
                        command.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;
                        command.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;
                        command.Parameters.Add("@LicenseClass", SqlDbType.Int).Value = LicenseClass;

                        if (Notes == "")
                            command.Parameters.Add("@Notes", SqlDbType.NVarChar, 500).Value = DBNull.Value;
                        else
                            command.Parameters.Add("@Notes", SqlDbType.NVarChar, 500).Value = Notes;

                        command.Parameters.Add("@PaidFees", SqlDbType.Float).Value = PaidFees;
                        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = IsActive;
                        command.Parameters.Add("@IssueReason", SqlDbType.TinyInt).Value = IssueReason;
                        command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = CreatedByUserID;

                        await connection.OpenAsync().ConfigureAwait(false);
                        rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to update license with LicenseID: {LicenseID}");
                return false;
            }

            return (rowsAffected > 0);
        }
      
        public static async Task<int> GetActiveLicenseIDByPersonIDAsync(int PersonID, int LicenseClassID)
        {
            int LicenseID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT CAST(Licenses.LicenseID AS INT)
                            FROM Licenses INNER JOIN
                                                     Drivers ON Licenses.DriverID = Drivers.DriverID
                            WHERE  
                             
                             Licenses.LicenseClass = @LicenseClass 
                              AND Drivers.PersonID = @PersonID
                              And IsActive=1;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;
                        command.Parameters.Add("@LicenseClass", SqlDbType.Int).Value = LicenseClassID;


                        await connection.OpenAsync().ConfigureAwait(false);

                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);

                        if (result is int retrievedID)
                        {
                            LicenseID = retrievedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve active LicenseID for PersonID: {PersonID}, LicenseClassID: {LicenseClassID}");
            }

            return LicenseID;
        }

        public static async Task<int> GetLicenseIDByApplicationIDAsync(int ApplicationID)
        {
            int LicenseID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT [LicenseID]
                             FROM [dbo].[Licenses]
                             WHERE ApplicationID= @ApplicationID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;

                        await connection.OpenAsync().ConfigureAwait(false);

                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);

                        if (result is int retrievedID)
                        {
                            LicenseID = retrievedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve LicenseID for ApplicationID: {ApplicationID}");
               
            }

            return LicenseID;
        }

        public static async Task<int> GetLastLicenseIDByDriverIDAsync(int DriverID, int LicenseClassID)
        {
            int lastLicenseID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT TOP (1) LicenseID
                              FROM Licenses
                              WHERE DriverID = @DriverID AND LicenseClass = @LicenseClassID
                              ORDER BY LicenseID DESC;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;
                        command.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;

                        await connection.OpenAsync().ConfigureAwait(false);
                        object scalar = await command.ExecuteScalarAsync().ConfigureAwait(false);

                        if (scalar is int retrievedID)
                        {
                            lastLicenseID = retrievedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve last LicenseID for DriverID: {DriverID}, LicenseClassID: {LicenseClassID}");
                
            }

            return lastLicenseID;
        }

        public static async Task<int> GetActiveLicenseIDByDriverIDAsync(int DriverID, int LicenseClassID)
        {
            int LicenseID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT        Licenses.LicenseID
                            FROM Licenses INNER JOIN
                                                     Drivers ON Licenses.DriverID = Drivers.DriverID
                            WHERE  
                             
                             Licenses.LicenseClass = @LicenseClass 
                              AND Drivers.DriverID = @DriverID
                              And IsActive=1;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;
                        command.Parameters.Add("@LicenseClass", SqlDbType.Int).Value = LicenseClassID;

                        await connection.OpenAsync().ConfigureAwait(false);

                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);

                        if (result is int retrievedID)
                        {
                            LicenseID = retrievedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve active LicenseID for DriverID: {DriverID}, LicenseClassID: {LicenseClassID}");
            }

            return LicenseID;
        }

        public static async Task<int> GetLicenseIDByDriverIDAsync(int DriverID, int LicenseClassID)
        {
            int LicenseID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT        Licenses.LicenseID
                            FROM Licenses INNER JOIN
                                                     Drivers ON Licenses.DriverID = Drivers.DriverID
                            WHERE  
                             
                             Licenses.LicenseClass = @LicenseClass 
                              AND Drivers.DriverID = @DriverID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;
                        command.Parameters.Add("@LicenseClass", SqlDbType.Int).Value = LicenseClassID;

                        await connection.OpenAsync().ConfigureAwait(false);

                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);

                        if (result is int retrievedID)
                        {
                            LicenseID = retrievedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve LicenseID for DriverID: {DriverID}, LicenseClassID: {LicenseClassID}");
            }

            return LicenseID;
        }


        public static async Task<bool> DeactivateLicenseIDByDriverIDAsync(int DriverID, int LicenseClassID)
        {
            int effectedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = "UPDATE Licenses " +
                        "SET IsActive = 0 " +
                        "WHERE LicenseID = ( " +
                        "SELECT TOP(1) LicenseID " +
                        "FROM Licenses " +
                        "WHERE DriverID = @DriverID " +
                        "AND LicenseClass = @LicenseClass " +
                        "ORDER BY LicenseID DESC " +
                        "); ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;
                        command.Parameters.Add("@LicenseClass", SqlDbType.Int).Value = LicenseClassID;

                        await connection.OpenAsync().ConfigureAwait(false);

                        effectedRows = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to deactivate license for DriverID: {DriverID}, LicenseClassID: {LicenseClassID}");
            }

            return (effectedRows > 0);
        }

        public static async Task<bool> DeactivateLicenseAsync(int LicenseID)
        {

            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"UPDATE Licenses
                           SET 
                              IsActive = 0
                             
                         WHERE LicenseID=@LicenseID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LicenseID", SqlDbType.Int).Value = LicenseID;

                        await connection.OpenAsync().ConfigureAwait(false);
                        rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to deactivate license with LicenseID: {LicenseID}");
            }

            return (rowsAffected > 0);
        }

        public static async Task<bool> IsLicenseActiveAsync(int LicenseID)
        {
            bool isActive = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT [IsActive]
                             FROM [dbo].[Licenses]
                             WHERE LicenseID = @LicenseID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LicenseID", SqlDbType.Int).Value = LicenseID;

                        await connection.OpenAsync().ConfigureAwait(false);

                        object result = await command.ExecuteScalarAsync().ConfigureAwait(false);

                        if (result != null)
                        {
                            isActive = (bool)result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check active status for LicenseID: {LicenseID}");
            }

            return isActive;
        }

        public static async Task<bool> DeactivateExpiredLicensesAsync()
        {

            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"UPDATE [dbo].[Licenses]
                             SET [IsActive] = 0
                             WHERE ExpirationDate < GETDATE() 
                               AND IsActive = 1;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync().ConfigureAwait(false);
                        rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Failed to deactivate expired licenses.");
            }

            return (rowsAffected > 0);
        }
    }
}
