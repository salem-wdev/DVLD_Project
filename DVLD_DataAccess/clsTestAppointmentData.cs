using DVLD_Shared;
using DVLD_Shared;
using DVLD_Infrastructure.Storage;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace DVLD_DataAccess
{
    public class clsTestAppointmentData
    {

        public static async Task<(bool isFound, int TestTypeID, int LocalDrivingLicenseApplicationID,
            DateTime AppointmentDate, float PaidFees, int CreatedByUserID, bool IsLocked, int RetakeTestApplicationID)>
            GetTestAppointmentInfoByIDAsync(int TestAppointmentID)
        {
            bool isFound = false;
            int TestTypeID = -1;
            int LocalDrivingLicenseApplicationID = -1;
            DateTime AppointmentDate = DateTime.MinValue;
            float PaidFees = 0;
            int CreatedByUserID = -1;
            bool IsLocked = false;
            int RetakeTestApplicationID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT TestTypeID, LocalDrivingLicenseApplicationID, AppointmentDate, CAST(PaidFees AS REAL) AS PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID
                                     FROM TestAppointments
                                     WHERE TestAppointmentID = @TestAppointmentID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = TestAppointmentID;

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
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

            return (isFound, TestTypeID, LocalDrivingLicenseApplicationID, AppointmentDate, PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID);
        }

        public static async Task<(bool isFound, int TestAppointmentID, DateTime AppointmentDate, float PaidFees, int CreatedByUserID, bool IsLocked, int RetakeTestApplicationID)> GetTestAppointmentInfoByLocalDrivingLicenseApplicationIDAsync(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            bool isFound = false;
            int TestAppointmentID = -1;
            DateTime AppointmentDate = DateTime.MinValue;
            float PaidFees = 0;
            int CreatedByUserID = -1;
            bool IsLocked = false;
            int RetakeTestApplicationID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
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

            return (isFound, TestAppointmentID, AppointmentDate, PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID);
        }

        public static async Task<(bool isFound, int TestAppointmentID, DateTime AppointmentDate, float PaidFees, int CreatedByUserID, bool IsLocked, int RetakeTestApplicationID)> GetLastTestAppointmentAsync(
             int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            bool isFound = false;
            int TestAppointmentID = -1;
            DateTime AppointmentDate = DateTime.MinValue;
            float PaidFees = 0;
            int CreatedByUserID = -1;
            bool IsLocked = false;
            int RetakeTestApplicationID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
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

            return (isFound, TestAppointmentID, AppointmentDate, PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID);
        }

        public static async Task<DataTable> GetAllTestAppointmentsAsync()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT TestAppointmentID, TestTypeID, LocalDrivingLicenseApplicationID, AppointmentDate, CAST(PaidFees AS REAL) AS PaidFees, CreatedByUserID, IsLocked, RetakeTestApplicationID
                                     FROM TestAppointments
                                     ORDER BY AppointmentDate DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync().ConfigureAwait(false);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
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

        public static async Task<DataTable> GetApplicationTestAppointmentsPerTestTypeAsync(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
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

                        await connection.OpenAsync().ConfigureAwait(false);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
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

        public static async Task<int> AddNewTestAppointmentAsync(
             int TestTypeID, int LocalDrivingLicenseApplicationID,
             DateTime AppointmentDate, float PaidFees, int CreatedByUserID, int RetakeTestApplicationID)
        {
            int TestAppointmentID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
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

                        await connection.OpenAsync().ConfigureAwait(false);

                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is int insertedID)
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

        public static async Task<bool> UpdateTestAppointmentAsync(int TestAppointmentID, int TestTypeID, int LocalDrivingLicenseApplicationID,
             DateTime AppointmentDate, float PaidFees,
             int CreatedByUserID, bool IsLocked, int RetakeTestApplicationID)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
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


        public static async Task<int> GetTestIDAsync(int TestAppointmentID)
        {
            int TestID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT TestID FROM Tests WHERE TestAppointmentID = @TestAppointmentID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = TestAppointmentID;

                        await connection.OpenAsync().ConfigureAwait(false);

                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is int testID)
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

        public static async Task<bool> DoesHaveAnActiveAppointmentAsync(int TestTypeID,
            int LocalDrivingLicenseApplicationID)
        {
            bool IsFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
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

                        await connection.OpenAsync().ConfigureAwait(false);

                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is bool found)
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

        public static async Task<bool> GetIsAppointmentLockedAsync(int TestTypeID,
            int LocalDrivingLicenseApplicationID)
        {
            bool IsLocked = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
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

                        await connection.OpenAsync().ConfigureAwait(false);

                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is bool isLocked)
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

        public static async Task<bool> GetIsAppointmentLockedByIDAsync(int TestAppointmentID)
        {
            bool IsLocked = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT TOP (1) IsLocked
                                     FROM [DVLD].[dbo].[TestAppointments]
                                     WHERE TestAppointmentID = @TestAppointmentID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = TestAppointmentID;

                        await connection.OpenAsync().ConfigureAwait(false);

                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is bool isLocked)
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

        public static async Task<bool> LockExpiredAppointmentsAsync(int TestTypeID,
            int LocalDrivingLicenseApplicationID)
        {
            bool IsLocked = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
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

                        await connection.OpenAsync().ConfigureAwait(false);

                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is bool isLocked)
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

        public static async Task<bool> GetIsAppointmentexistsAsync(int TestTypeID,
            int LocalDrivingLicenseApplicationID)
        {
            bool IsFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
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

                        await connection.OpenAsync().ConfigureAwait(false);

                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is bool found)
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

        public static async Task<bool> LockExpiredTestAppointmentsAsync()
        {
            int EffectedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"UPDATE [dbo].[TestAppointments]
                                     SET IsLocked = 1
                                     WHERE AppointmentDate < GETDATE()
                                       AND IsLocked = 0;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync().ConfigureAwait(false);
                        EffectedRows = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
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
