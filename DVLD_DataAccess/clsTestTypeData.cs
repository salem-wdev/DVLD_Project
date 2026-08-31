using DVLD_Shared;
using DVLD_Infrastructure.Storage;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsTestTypeData
    {
        public static async Task<(bool IsFound, string TestTypeTitle, string TestTypeDescription, decimal TestTypeFees)>
            GetTestTypeInfoByIDAsync(int TestTypeID)
        {
            bool IsFound = false;
            string TestTypeTitle = string.Empty;
            string TestTypeDescription = string.Empty;
            decimal TestTypeFees = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"SELECT TestTypeTitle, TestTypeDescription, TestTypeFees
                                     FROM TestTypes
                                     WHERE TestTypeID = @TestTypeID;";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                TestTypeTitle = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                                TestTypeDescription = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                                TestTypeFees = reader.GetDecimal(2);
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
                clsLogger.LogException(ex, $"Failed to retrieve test type info by ID: {TestTypeID}");
                IsFound = false;
            }

            return (IsFound, TestTypeTitle, TestTypeDescription, TestTypeFees);
        }

        public static async Task<int> AddNewTestTypeAsync(string TestTypeTitle,
           string TestTypeDescription, decimal TestTypeFees)
        {
            int TestTypeID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"INSERT INTO TestTypes (TestTypeTitle, TestTypeDescription, TestTypeFees)
                                     VALUES (@TestTypeTitle, @TestTypeDescription, @TestTypeFees);
                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@TestTypeTitle", SqlDbType.NVarChar).Value = TestTypeTitle;
                        command.Parameters.Add("@TestTypeDescription", SqlDbType.NVarChar).Value = TestTypeDescription;
                        command.Parameters.Add("@TestTypeFees", SqlDbType.Decimal).Value = TestTypeFees;

                        await connection.OpenAsync().ConfigureAwait(false);
                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is int NewID)
                        {
                            TestTypeID = NewID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to add new test type: {TestTypeTitle}");
            }

            return TestTypeID;
        }

        public static async Task<bool> UpdateTestTypeAsync(int TestTypeID,
        string TestTypeTitle, string TestTypeDescription, decimal TestTypeFees)
        {
            int NumberOfEffectedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"UPDATE TestTypes
                                     SET TestTypeTitle = @TestTypeTitle,
                                         TestTypeDescription = @TestTypeDescription,
                                         TestTypeFees = @TestTypeFees
                                     WHERE TestTypeID = @TestTypeID";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        Command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;
                        Command.Parameters.Add("@TestTypeDescription", SqlDbType.NVarChar).Value = TestTypeDescription;
                        Command.Parameters.Add("@TestTypeTitle", SqlDbType.NVarChar).Value = TestTypeTitle;
                        Command.Parameters.Add("@TestTypeFees", SqlDbType.Decimal).Value = TestTypeFees;

                        await connection.OpenAsync().ConfigureAwait(false);
                        NumberOfEffectedRows = await Command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to update test type ID: {TestTypeID}");
            }

            return NumberOfEffectedRows > 0;
        }

        public static async Task<DataTable> GetAllTestTypesAsync()
        {
            DataTable Table = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"SELECT TestTypeID, TestTypeTitle, TestTypeDescription, TestTypeFees
                                     FROM TestTypes";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await Command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (reader.HasRows)
                            {
                                Table.Load(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Failed to retrieve all test types.");
            }

            return Table;
        }

    }
}
