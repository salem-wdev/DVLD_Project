using DVLD_Shared;
using DVLD_Infrastructure.Storage;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public class clsTestData
    {
        public static bool GetTestInfoByID(int TestID,
            ref int TestAppointmentID, ref bool TestResult,
            ref string Notes, ref int CreatedByUserID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT TestAppointmentID, TestResult, Notes, CreatedByUserID
                                     FROM Tests
                                     WHERE TestID = @TestID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@TestID", SqlDbType.Int).Value = TestID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                TestAppointmentID = reader.GetInt32(0);
                                TestResult = reader.GetBoolean(1);
                                Notes = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                                CreatedByUserID = reader.GetInt32(3);
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
                clsLogger.LogException(ex, $"Failed to retrieve test info by ID: {TestID}");
                isFound = false;
            }

            return isFound;
        }

        public static bool GetLastTestByPersonAndTestTypeAndLicenseClass
            (int PersonID, int LicenseClassID, int TestTypeID, ref int TestID,
              ref int TestAppointmentID, ref bool TestResult,
              ref string Notes, ref int CreatedByUserID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT TOP (1) Tests.TestID,
                                            Tests.TestAppointmentID,
                                            Tests.TestResult,
                                            Tests.Notes,
                                            Tests.CreatedByUserID
                                     FROM LocalDrivingLicenseApplications
                                     INNER JOIN TestAppointments
                                         ON LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID
                                     INNER JOIN Tests
                                         ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
                                     INNER JOIN Applications
                                         ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID
                                     WHERE Applications.ApplicantPersonID = @PersonID
                                       AND LocalDrivingLicenseApplications.LicenseClassID = @LicenseClassID
                                       AND TestAppointments.TestTypeID = @TestTypeID
                                     ORDER BY Tests.TestAppointmentID DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;
                        command.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;
                        command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                TestID = reader.GetInt32(0);
                                TestAppointmentID = reader.GetInt32(1);
                                TestResult = reader.GetBoolean(2);
                                Notes = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                CreatedByUserID = reader.GetInt32(4);
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
                clsLogger.LogException(ex, $"Failed to retrieve last test for PersonID: {PersonID}, LicenseClassID: {LicenseClassID}, TestTypeID: {TestTypeID}");
                isFound = false;
            }

            return isFound;
        }

        public static DataTable GetAllTests()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT TestID, TestAppointmentID, TestResult, Notes, CreatedByUserID
                                     FROM Tests
                                     ORDER BY TestID";

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
                clsLogger.LogException(ex, "Failed to retrieve all tests.");
            }

            return dt;
        }

        public static int AddNewTest(int TestAppointmentID, bool TestResult,
             string Notes, int CreatedByUserID)
        {
            int TestID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"INSERT INTO Tests (TestAppointmentID, TestResult, Notes, CreatedByUserID)
                                     VALUES (@TestAppointmentID, @TestResult, @Notes, @CreatedByUserID);

                                     UPDATE TestAppointments
                                     SET IsLocked = 1
                                     WHERE TestAppointmentID = @TestAppointmentID;

                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = TestAppointmentID;
                        command.Parameters.Add("@TestResult", SqlDbType.Bit).Value = TestResult;
                        command.Parameters.Add("@Notes", SqlDbType.NVarChar, 500).Value = !string.IsNullOrEmpty(Notes) ? (object)Notes : DBNull.Value;
                        command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = CreatedByUserID;

                        connection.Open();

                        if (command.ExecuteScalar() is int insertedID)
                        {
                            TestID = insertedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to add new test for TestAppointmentID: {TestAppointmentID}");
            }

            return TestID;
        }

        public static bool UpdateTest(int TestID, int TestAppointmentID, bool TestResult,
             string Notes, int CreatedByUserID)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"UPDATE Tests
                                     SET TestAppointmentID = @TestAppointmentID,
                                         TestResult = @TestResult,
                                         Notes = @Notes,
                                         CreatedByUserID = @CreatedByUserID
                                     WHERE TestID = @TestID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@TestID", SqlDbType.Int).Value = TestID;
                        command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = TestAppointmentID;
                        command.Parameters.Add("@TestResult", SqlDbType.Bit).Value = TestResult;
                        command.Parameters.Add("@Notes", SqlDbType.NVarChar, 500).Value = (object)Notes ?? DBNull.Value;
                        command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = CreatedByUserID;

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to update test ID: {TestID}");
                return false;
            }

            return (rowsAffected > 0);
        }

        public static byte GetPassedTestCount(int LocalDrivingLicenseApplicationID)
        {
            byte PassedTestCount = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT CAST(COUNT(TestTypeID) AS TINYINT)
                                     FROM Tests
                                     INNER JOIN TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
                                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                       AND TestResult = 1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;

                        connection.Open();

                        if (command.ExecuteScalar() is byte ptCount)
                        {
                            PassedTestCount = ptCount;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve passed test count for LocalDrivingLicenseApplicationID: {LocalDrivingLicenseApplicationID}");
            }

            return PassedTestCount;
        }

        public static bool GetIsPassedTestByTestAppointmentID(int TestAppointmentID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT TOP (1) CAST(1 AS BIT)
                                     FROM Tests
                                     WHERE Tests.TestAppointmentID = @TestAppointmentID
                                       AND Tests.TestResult = 1;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = TestAppointmentID;

                        connection.Open();

                        if (command.ExecuteScalar() is bool found)
                        {
                            isFound = found;
                        }
                        else
                        {
                            isFound = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check passed test by TestAppointmentID: {TestAppointmentID}");
                isFound = false;
            }

            return isFound;
        }


    }
}
