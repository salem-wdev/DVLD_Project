using DVLD_Shared;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public class clsLocalDrivingLicenseApplicationData
    {
        public static bool GetLocalDrivingLicenseApplicationInfoByID(
            int LocalDrivingLicenseApplicationID, ref int ApplicationID,
            ref int LicenseClassID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT ApplicationID, LicenseClassID FROM LocalDrivingLicenseApplications WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                ApplicationID = reader.GetInt32(0);
                                LicenseClassID = reader.GetInt32(1);
                            }
                            else
                            {
                                isFound = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve local driving license application info by ID: {LocalDrivingLicenseApplicationID}");
                isFound = false;
            }

            return isFound;
        }

        public static bool GetLocalDrivingLicenseApplicationInfoByApplicationID(
         int ApplicationID, ref int LocalDrivingLicenseApplicationID,
         ref int LicenseClassID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT LocalDrivingLicenseApplicationID, LicenseClassID FROM LocalDrivingLicenseApplications WHERE ApplicationID = @ApplicationID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                LocalDrivingLicenseApplicationID = reader.GetInt32(0);
                                LicenseClassID = reader.GetInt32(1);
                            }
                            else
                            {
                                isFound = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve local driving license application info by ApplicationID: {ApplicationID}");
                isFound = false;
            }

            return isFound;
        }

        public static DataTable GetAllLocalDrivingLicenseApplications()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT *
                              FROM LocalDrivingLicenseApplications_View
                              order by ApplicationDate Desc";

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
                clsLogger.LogException(ex, "Failed to retrieve all local driving license applications.");
            }

            return dt;
        }

        public static int AddNewLocalDrivingLicenseApplication(
            int ApplicationID, int LicenseClassID)
        {
            int LocalDrivingLicenseApplicationID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"INSERT INTO LocalDrivingLicenseApplications ( 
                            ApplicationID,LicenseClassID)
                             VALUES (@ApplicationID,@LicenseClassID);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;
                        command.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;

                        connection.Open();

                        if (command.ExecuteScalar() is int insertedID)
                        {
                            LocalDrivingLicenseApplicationID = insertedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to add local driving license application for ApplicationID: {ApplicationID}, LicenseClassID: {LicenseClassID}");
            }

            return LocalDrivingLicenseApplicationID;
        }

        public static bool UpdateLocalDrivingLicenseApplication(
            int LocalDrivingLicenseApplicationID, int ApplicationID, int LicenseClassID)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"Update  LocalDrivingLicenseApplications  
                            set ApplicationID = @ApplicationID,
                                LicenseClassID = @LicenseClassID
                            where LocalDrivingLicenseApplicationID=@LocalDrivingLicenseApplicationID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        command.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;
                        command.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to update local driving license application ID: {LocalDrivingLicenseApplicationID}");
                return false;
            }

            return (rowsAffected > 0);
        }

        public static bool DeleteLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"Delete LocalDrivingLicenseApplications 
                                where LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to delete local driving license application ID: {LocalDrivingLicenseApplicationID}");
            }

            return (rowsAffected > 0);
        }

        public static byte TotalTrialsPerTest(int LocalDrivingLicenseApplicationID, int TestTypeID)

        {
            byte TotalTrialsPerTest = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @" SELECT CAST(COUNT(TestID) AS TINYINT)
                            FROM LocalDrivingLicenseApplications INNER JOIN
                                 TestAppointments ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID INNER JOIN
                                 Tests ON TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                            WHERE
                            (LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID) 
                            AND(TestAppointments.TestTypeID = @TestTypeID)
                       ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;

                        connection.Open();

                        if (command.ExecuteScalar() is byte trials)
                        {
                            TotalTrialsPerTest = trials;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to get total trials for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}, TestTypeID: {TestTypeID}");
            }

            return TotalTrialsPerTest;

        }

        public static bool DoesPassTestType(int LocalDrivingLicenseApplicationID, int TestTypeID)

        {
            bool Result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @" SELECT top 1 TestResult
                            FROM LocalDrivingLicenseApplications INNER JOIN
                                 TestAppointments ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID INNER JOIN
                                 Tests ON TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                            WHERE
                            (LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID) 
                            AND(TestAppointments.TestTypeID = @TestTypeID)
                            ORDER BY TestAppointments.TestAppointmentID desc";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;

                        connection.Open();

                        if (command.ExecuteScalar() is bool returnedResult)
                        {
                            Result = returnedResult;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to evaluate test pass status for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}, TestTypeID: {TestTypeID}");
            }

            return Result;

        }

        public static bool DoesPassAllTests(int LocalDrivingLicenseApplicationID)
        {
            bool Result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = " SELECT TestResult " +
                        "FROM Tests " +
                        "WHERE (TestAppointmentID = " +
                        "(SELECT TOP (1) TestAppointmentID " +
                        "FROM TestAppointments " +
                        "WHERE (LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID AND TestTypeID = 3) " +
                        "ORDER BY TestAppointmentID DESC))";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;

                        connection.Open();

                        if (command.ExecuteScalar() is bool returnedResult)
                        {
                            Result = returnedResult;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to evaluate all tests pass status for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}");
            }

            return Result;

        }

        public static bool GetIsLocalDrivingLicenseApplicationHasLicense(int LocalDrivingLicenseApplicationID, int LicenseClassID)
        {
            bool HasLicense = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TOP (1) CAST(1 AS BIT) 
                            FROM   Licenses L 
                            INNER JOIN LocalDrivingLicenseApplications 
                                ON L.ApplicationID = LocalDrivingLicenseApplications.ApplicationID
                            WHERE  LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID 
                              AND  LocalDrivingLicenseApplications.LicenseClassID = @LicenseClassID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        command.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;

                        connection.Open();

                        HasLicense = command.ExecuteScalar() is bool;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check existing license for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}, LicenseClassID: {LicenseClassID}");
            }

            return HasLicense;
        }

        public static bool IsLocalDrivingLicenseApplicationHasActiveTestAppointment(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            bool HasActiveAppointment = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TOP (1) CAST(1 AS BIT)
                             FROM            LocalDrivingLicenseApplications INNER JOIN
                             TestAppointments ON
                             LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID 
                             = TestAppointments.LocalDrivingLicenseApplicationID
                             WHERE LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID =
                             @LocalDrivingLicenseApplicationID
                             AND TestTypeID = @TestTypeID AND IsLocked = 0;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;

                        connection.Open();

                        if (command.ExecuteScalar() is bool hasActiveAppointment)
                        {
                            HasActiveAppointment = hasActiveAppointment;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check active test appointment for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}, TestTypeID: {TestTypeID}");
            }

            return HasActiveAppointment;
        }

        public static int GetApplicationStatus(int LocalDrivingLicenseApplicationID)
        {
            int ApplicationStatus = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TOP (1) CAST(Applications.ApplicationStatus AS TINYINT)
                              FROM            Applications INNER JOIN
                              LocalDrivingLicenseApplications ON Applications.ApplicationID = LocalDrivingLicenseApplications.ApplicationID 
                              AND LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;

                        connection.Open();

                        if (command.ExecuteScalar() is byte AppID)
                        {
                            ApplicationStatus = AppID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve application status for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}");
            }

            return ApplicationStatus;
        }

        public static bool DoesAttendTestType(int LocalDrivingLicenseApplicationID, int TestTypeID)

        {
            bool IsFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @" SELECT top (1)  CAST(1 AS BIT)
                            FROM LocalDrivingLicenseApplications INNER JOIN
                                 TestAppointments ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID INNER JOIN
                                 Tests ON TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                            WHERE
                            (LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID) 
                            AND(TestAppointments.TestTypeID = @TestTypeID)
                            ORDER BY TestAppointments.TestAppointmentID desc";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;

                        connection.Open();

                        if (command.ExecuteScalar() is bool found)
                        {
                            IsFound = found;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check attended test type for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}, TestTypeID: {TestTypeID}");
            }

            return IsFound;

        }
    }
}
