using DVLD_Shared;
using System;
using System.Data;
using System.Data.SqlClient;


namespace DVLD_DataAccess
{
    public class clsTestAppointmentData
    {

        public static bool GetTestAppointmentInfoByID(int TestAppointmentID,
            ref int TestTypeID, ref int LocalDrivingLicenseApplicationID,
            ref DateTime AppointmentDate, ref float PaidFees, ref int CreatedByUserID, ref bool IsLocked, ref int RetakeTestApplicationID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TestTypeID, LocalDrivingLicenseApplicationID, AppointmentDate, CAST(PaidFees AS REAL) AS PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID
                                     FROM TestAppointments
                                     WHERE TestAppointmentID = @TestAppointmentID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = TestAppointmentID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                TestTypeID = reader.GetInt32(0);
                                LocalDrivingLicenseApplicationID = reader.GetInt32(1);
                                AppointmentDate = reader.GetDateTime(2);
                                PaidFees = reader.GetFloat(3);
                                CreatedByUserID = reader.GetInt32(4);
                                IsLocked = reader.GetBoolean(5);
                                RetakeTestApplicationID = reader.IsDBNull(6) ? -1 : reader.GetInt32(6);
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
                clsLogger.LogException(ex, $"Failed to retrieve test appointment info by ID: {TestAppointmentID}");
                isFound = false;
            }

            return isFound;
        }

        public static bool GetTestAppointmentInfoByLocalDrivingLicenseApplicationID(int LocalDrivingLicenseApplicationID, int TestTypeID,
            ref int TestAppointmentID, ref DateTime AppointmentDate,
            ref float PaidFees, ref int CreatedByUserID, ref bool IsLocked,
            ref int RetakeTestApplicationID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TOP (1) TestAppointmentID, AppointmentDate, CAST(PaidFees AS REAL) AS PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID
                                     FROM TestAppointments
                                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                       AND TestTypeID = @TestTypeID
                                     ORDER BY TestAppointmentID DESC;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                TestAppointmentID = reader.GetInt32(0);
                                AppointmentDate = reader.GetDateTime(1);
                                PaidFees = reader.GetFloat(2);
                                CreatedByUserID = reader.GetInt32(3);
                                IsLocked = reader.GetBoolean(4);
                                RetakeTestApplicationID = reader.IsDBNull(5) ? -1 : reader.GetInt32(5);
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
                clsLogger.LogException(ex, $"Failed to retrieve test appointment info by LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}, TestTypeID: {TestTypeID}");
                isFound = false;
            }

            return isFound;
        }

        public static bool GetLastTestAppointment(
             int LocalDrivingLicenseApplicationID, int TestTypeID,
            ref int TestAppointmentID, ref DateTime AppointmentDate,
            ref float PaidFees, ref int CreatedByUserID, ref bool IsLocked, ref int RetakeTestApplicationID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TOP (1) TestAppointmentID, AppointmentDate, CAST(PaidFees AS REAL) AS PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID
                                     FROM TestAppointments
                                     WHERE TestTypeID = @TestTypeID
                                       AND LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                     ORDER BY TestAppointmentID DESC;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                TestAppointmentID = reader.GetInt32(0);
                                AppointmentDate = reader.GetDateTime(1);
                                PaidFees = reader.GetFloat(2);
                                CreatedByUserID = reader.GetInt32(3);
                                IsLocked = reader.GetBoolean(4);
                                RetakeTestApplicationID = reader.IsDBNull(5) ? -1 : reader.GetInt32(5);
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
                clsLogger.LogException(ex, $"Failed to retrieve last test appointment for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}, TestTypeID: {TestTypeID}");
                isFound = false;
            }

            return isFound;
        }

        public static DataTable GetAllTestAppointments()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TestAppointmentID, TestTypeID, LocalDrivingLicenseApplicationID, AppointmentDate, CAST(PaidFees AS REAL) AS PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID
                                     FROM TestAppointments
                                     ORDER BY AppointmentDate DESC";

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
                clsLogger.LogException(ex, "Failed to retrieve all test appointments.");
            }

            return dt;
        }

        public static DataTable GetApplicationTestAppointmentsPerTestType(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TestAppointmentID, AppointmentDate, CAST(PaidFees AS REAL) AS PaidFees, IsLocked
                                     FROM TestAppointments
                                     WHERE TestTypeID = @TestTypeID
                                       AND LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                     ORDER BY TestAppointmentID DESC;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;

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
                clsLogger.LogException(ex, $"Failed to retrieve test appointments for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}, TestTypeID: {TestTypeID}");
            }

            return dt;
        }

        public static int AddNewTestAppointment(
             int TestTypeID, int LocalDrivingLicenseApplicationID,
             DateTime AppointmentDate, float PaidFees, int CreatedByUserID, int RetakeTestApplicationID)
        {
            int TestAppointmentID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"INSERT INTO TestAppointments (TestTypeID, LocalDrivingLicenseApplicationID, AppointmentDate, PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID)
                                     VALUES (@TestTypeID, @LocalDrivingLicenseApplicationID, @AppointmentDate, @PaidFees, @CreatedByUserID, 0, @RetakeTestApplicationID);
                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        command.Parameters.Add("@AppointmentDate", SqlDbType.DateTime).Value = AppointmentDate;
                        command.Parameters.Add("@PaidFees", SqlDbType.Real).Value = PaidFees;
                        command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = CreatedByUserID;
                        command.Parameters.Add("@RetakeTestApplicationID", SqlDbType.Int).Value = RetakeTestApplicationID == -1 ? (object)DBNull.Value : RetakeTestApplicationID;

                        connection.Open();

                        if (command.ExecuteScalar() is int insertedID)
                        {
                            TestAppointmentID = insertedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to add test appointment for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}, TestTypeID: {TestTypeID}");
            }

            return TestAppointmentID;
        }

        public static bool UpdateTestAppointment(int TestAppointmentID, int TestTypeID, int LocalDrivingLicenseApplicationID,
             DateTime AppointmentDate, float PaidFees,
             int CreatedByUserID, bool IsLocked, int RetakeTestApplicationID)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE TestAppointments
                                     SET TestTypeID = @TestTypeID,
                                         LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID,
                                         AppointmentDate = @AppointmentDate,
                                         PaidFees = @PaidFees,
                                         CreatedByUserID = @CreatedByUserID,
                                         IsLocked = @IsLocked,
                                         RetakeTestApplicationID = @RetakeTestApplicationID
                                     WHERE TestAppointmentID = @TestAppointmentID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = TestAppointmentID;
                        command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        command.Parameters.Add("@AppointmentDate", SqlDbType.DateTime).Value = AppointmentDate;
                        command.Parameters.Add("@PaidFees", SqlDbType.Real).Value = PaidFees;
                        command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = CreatedByUserID;
                        command.Parameters.Add("@IsLocked", SqlDbType.Bit).Value = IsLocked;
                        command.Parameters.Add("@RetakeTestApplicationID", SqlDbType.Int).Value = RetakeTestApplicationID == -1 ? (object)DBNull.Value : RetakeTestApplicationID;

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to update test appointment ID: {TestAppointmentID}");
                return false;
            }

            return (rowsAffected > 0);
        }


        public static int GetTestID(int TestAppointmentID)
        {
            int TestID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TestID FROM Tests WHERE TestAppointmentID = @TestAppointmentID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = TestAppointmentID;

                        connection.Open();

                        if (command.ExecuteScalar() is int testID)
                        {
                            TestID = testID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve TestID for TestAppointmentID: {TestAppointmentID}");
            }

            return TestID;
        }

        public static bool DoesHaveAnActiveAppointment(int TestTypeID,
            int LocalDrivingLicenseApplicationID)
        {
            bool IsFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TOP (1) CAST(1 AS BIT)
                                     FROM [TestAppointments]
                                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                       AND [TestTypeID] = @TestTypeID
                                       AND [IsLocked] = 0
                                     ORDER BY TestAppointmentID DESC;";

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
                clsLogger.LogException(ex, $"Failed to check active appointment for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}, TestTypeID: {TestTypeID}");
                IsFound = false;
            }

            return IsFound;
        }

        public static bool GetIsAppointmentLocked(int TestTypeID,
            int LocalDrivingLicenseApplicationID)
        {
            bool IsLocked = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TOP (1) IsLocked
                                     FROM [TestAppointments]
                                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                       AND [TestTypeID] = @TestTypeID
                                     ORDER BY TestAppointmentID DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;

                        connection.Open();

                        if (command.ExecuteScalar() is bool isLocked)
                        {
                            IsLocked = isLocked;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve appointment lock status for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}, TestTypeID: {TestTypeID}");
                IsLocked = false;
            }

            return IsLocked;
        }

        public static bool GetIsAppointmentLockedByID(int TestAppointmentID)
        {
            bool IsLocked = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TOP (1) IsLocked
                                     FROM [DVLD].[dbo].[TestAppointments]
                                     WHERE TestAppointmentID = @TestAppointmentID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = TestAppointmentID;

                        connection.Open();

                        if (command.ExecuteScalar() is bool isLocked)
                        {
                            IsLocked = isLocked;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve appointment lock status for TestAppointmentID: {TestAppointmentID}");
                IsLocked = false;
            }

            return IsLocked;
        }

        public static bool LockExpiredAppointments(int TestTypeID,
            int LocalDrivingLicenseApplicationID)
        {
            bool IsLocked = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE TestAppointments
                                     SET IsLocked = 1
                                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                       AND TestTypeID = @TestTypeID
                                       AND IsLocked = 0
                                       AND AppointmentDate < GETDATE();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;

                        connection.Open();

                        if (command.ExecuteScalar() is bool isLocked)
                        {
                            IsLocked = isLocked;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to lock expired appointments for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}, TestTypeID: {TestTypeID}");
                IsLocked = false;
            }

            return IsLocked;
        }

        public static bool GetIsAppointmentexists(int TestTypeID,
            int LocalDrivingLicenseApplicationID)
        {
            bool IsFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TOP (1) CAST(1 AS BIT)
                                     FROM [DVLD].[dbo].[TestAppointments]
                                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                       AND [TestTypeID] = @TestTypeID
                                     ORDER BY TestAppointmentID DESC";

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
                clsLogger.LogException(ex, $"Failed to check appointment existence for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}, TestTypeID: {TestTypeID}");
                IsFound = false;
            }

            return IsFound;
        }

        public static bool LockExpiredTestAppointments()
        {
            int EffectedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE [dbo].[TestAppointments]
                                     SET IsLocked = 1
                                     WHERE AppointmentDate < GETDATE()
                                       AND IsLocked = 0;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        EffectedRows = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Failed to lock expired test appointments.");
            }

            return EffectedRows > 0;
        }

    }
}
