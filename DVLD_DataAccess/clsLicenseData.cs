using DVLD_Shared;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace DVLD_DataAccess
{
    public class clsLicenseData
    {
        public static bool GetLicenseInfoByID(int LicenseID, ref int ApplicationID, ref int DriverID, ref int LicenseClass,
            ref DateTime IssueDate, ref DateTime ExpirationDate, ref string Notes,
            ref float PaidFees, ref bool IsActive, ref byte IssueReason, ref int CreatedByUserID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM Licenses WHERE LicenseID = @LicenseID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.Add("@LicenseID", SqlDbType.Int).Value = LicenseID;


                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
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

            return isFound;
        }

        public static bool GetLicenseInfoByApplicationID(int ApplicationID, ref int LicenseID, ref int DriverID, ref int LicenseClass,
    ref DateTime IssueDate, ref DateTime ExpirationDate, ref string Notes,
    ref float PaidFees, ref bool IsActive, ref byte IssueReason, ref int CreatedByUserID)
        {
            bool isFound = false;

            try
            {

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM Licenses WHERE ApplicationID = @ApplicationID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.Read())
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
            return isFound;
        }


        public static DataTable GetAllLicenses()
        {

            DataTable dt = new DataTable();
            try
            {   using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM Licenses";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
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

        public static DataTable GetDriverLicenses(int DriverID)
        {

            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
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
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
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

        public static int AddNewLicense(int ApplicationID, int DriverID, int LicenseClass,
            ref DateTime IssueDate, ref DateTime ExpirationDate, string Notes,
             float PaidFees, bool IsActive, byte IssueReason, int CreatedByUserID)
        {
            int LicenseID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
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
                                
                                
                                SELECT SCOPE_IDENTITY() AS NewLicenseID, @CurrentDate AS DB_IssueDate, @CalculatedExpirationDate AS DB_ExpirationDate;";
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

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                        {
                            if (reader.Read())
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
            return LicenseID;
        }

        public static bool UpdateLicense(int LicenseID, int ApplicationID, int DriverID, int LicenseClass,
             string Notes, float PaidFees, bool IsActive, byte IssueReason,
             int CreatedByUserID)
        {

            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
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
                            command.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = DBNull.Value;
                        else
                            command.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = Notes;

                        command.Parameters.Add("@PaidFees", SqlDbType.Float).Value = PaidFees;
                        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = IsActive;
                        command.Parameters.Add("@IssueReason", SqlDbType.TinyInt).Value = IssueReason;
                        command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = CreatedByUserID;

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
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

        public static int GetActiveLicenseIDByPersonID(int PersonID, int LicenseClassID)
        {
            int LicenseID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
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


                        connection.Open();

                        object result = command.ExecuteScalar();

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

        public static int GetLicenseIDByApplicationID(int ApplicationID)
        {
            int LicenseID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT [LicenseID]
                             FROM [dbo].[Licenses]
                             WHERE ApplicationID= @ApplicationID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                        connection.Open();

                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            LicenseID = insertedID;
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

        public static int GetLastLicenseIDByDriverID(int DriverID, int LicenseClassID)
        {
            int lastLicenseID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TOP (1) LicenseID
                              FROM Licenses
                              WHERE DriverID = @DriverID AND LicenseClass = @LicenseClassID
                              ORDER BY LicenseID DESC;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DriverID", DriverID);
                        command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                        connection.Open();
                        object scalar = command.ExecuteScalar();

                        if (scalar != null && int.TryParse(scalar.ToString(), out int parsedID))
                        {

                            lastLicenseID = parsedID;
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

        public static int GetActiveLicenseIDByDriverID(int DriverID, int LicenseClassID)
        {
            int LicenseID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
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
                        command.Parameters.AddWithValue("@DriverID", DriverID);
                        command.Parameters.AddWithValue("@LicenseClass", LicenseClassID);

                        connection.Open();

                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            LicenseID = insertedID;
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

        public static int GetLicenseIDByDriverID(int DriverID, int LicenseClassID)
        {
            int LicenseID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT        Licenses.LicenseID
                            FROM Licenses INNER JOIN
                                                     Drivers ON Licenses.DriverID = Drivers.DriverID
                            WHERE  
                             
                             Licenses.LicenseClass = @LicenseClass 
                              AND Drivers.DriverID = @DriverID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DriverID", DriverID);
                        command.Parameters.AddWithValue("@LicenseClass", LicenseClassID);

                        connection.Open();

                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            LicenseID = insertedID;
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


        public static bool DeactivateLicenseIDByDriverID(int DriverID, int LicenseClassID)
        {
            int effectedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
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
                        command.Parameters.AddWithValue("@DriverID", DriverID);
                        command.Parameters.AddWithValue("@LicenseClass", LicenseClassID);

                        connection.Open();

                        effectedRows = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to deactivate license for DriverID: {DriverID}, LicenseClassID: {LicenseClassID}");
            }

            return (effectedRows > 0);
        }

        public static bool DeactivateLicense(int LicenseID)
        {

            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE Licenses
                           SET 
                              IsActive = 0
                             
                         WHERE LicenseID=@LicenseID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LicenseID", LicenseID);

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to deactivate license with LicenseID: {LicenseID}");
            }

            return (rowsAffected > 0);
        }

        public static bool IsLicenseActive(int LicenseID)
        {
            bool isActive = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT [IsActive]
                             FROM [dbo].[Licenses]
                             WHERE LicenseID = @LicenseID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LicenseID", LicenseID);

                        connection.Open();

                        object result = command.ExecuteScalar();

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

        public static bool DeactivateExpiredLicenses()
        {

            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE [dbo].[Licenses]
                             SET [IsActive] = 0
                             WHERE ExpirationDate < GETDATE() 
                               AND IsActive = 1;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
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
