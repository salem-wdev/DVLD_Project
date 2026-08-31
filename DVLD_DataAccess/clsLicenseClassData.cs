using DVLD_Shared;
using DVLD_Infrastructure.Storage;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsLicenseClassData
    {
        public static async Task<(bool IsFound, string ClassName, string ClassDescription, byte MinimumAllowedAge, byte DefaultValidityLength, float ClassFees)> GetLicenseClassInfoByIDAsync(int LicenseClassID)
        {
            string ClassName = string.Empty;
            string ClassDescription = string.Empty;
            byte MinimumAllowedAge = 0;
            byte DefaultValidityLength = 0;
            float ClassFees = 0;
            bool isFound = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {

                    string query = "SELECT * FROM LicenseClasses WHERE LicenseClassID = @LicenseClassID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {

                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                // The record was found
                                isFound = true;

                                ClassName = (string)reader["ClassName"];
                                ClassDescription = (string)reader["ClassDescription"];
                                MinimumAllowedAge = (byte)reader["MinimumAllowedAge"];
                                DefaultValidityLength = (byte)reader["DefaultValidityLength"];
                                ClassFees = Convert.ToSingle(reader["ClassFees"]);

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
                clsLogger.LogException(ex, $"faild to get license class info by id = {LicenseClassID}");
            }

            return (isFound, ClassName, ClassDescription, MinimumAllowedAge, DefaultValidityLength, ClassFees);
        }


        public static async Task<(bool IsFound, int LicenseClassID, string ClassDescription, byte MinimumAllowedAge, byte DefaultValidityLength, float ClassFees)> GetLicenseClassInfoByClassNameAsync(string ClassName)
        {
            int LicenseClassID = -1;
            string ClassDescription = string.Empty;
            byte MinimumAllowedAge = 0;
            byte DefaultValidityLength = 0;
            float ClassFees = 0;
            bool isFound = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {

                    string query = "SELECT * FROM LicenseClasses WHERE ClassName = @ClassName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ClassName", ClassName);


                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {

                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                // The record was found
                                isFound = true;
                                LicenseClassID = (int)reader["LicenseClassID"];
                                ClassDescription = (string)reader["ClassDescription"];
                                MinimumAllowedAge = (byte)reader["MinimumAllowedAge"];
                                DefaultValidityLength = (byte)reader["DefaultValidityLength"];
                                ClassFees = Convert.ToSingle(reader["ClassFees"]);
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

                clsLogger.LogException(ex, $"faild to get license class info by class name = {ClassName}");
            }

            return (isFound, LicenseClassID, ClassDescription, MinimumAllowedAge, DefaultValidityLength, ClassFees);
        }

        public static async Task<DataTable> GetAllLicenseClassesAsync()
        {

            DataTable dt = new DataTable();

            dt.Columns.Add("LicenseClassID", typeof(int));
            dt.Columns.Add("ClassName", typeof(string));
            dt.Columns.Add("ClassDescription", typeof(string));
            dt.Columns.Add("MinimumAllowedAge", typeof(byte));
            dt.Columns.Add("DefaultValidityLength", typeof(byte));
            dt.Columns.Add("ClassFees", typeof(decimal));

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = "SELECT LicenseClassID, ClassName, ClassDescription, MinimumAllowedAge, DefaultValidityLength, ClassFees FROM LicenseClasses order by ClassName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync().ConfigureAwait(false);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                dt.Rows.Add(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetByte(3), reader.GetByte(4), reader.GetDecimal(5));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                clsLogger.LogException(ex, $"faild to get all license classes Table");
            }

            return dt;

        }

        public static async Task<int> AddNewLicenseClassAsync(string ClassName, string ClassDescription,
            byte MinimumAllowedAge, byte DefaultValidityLength, float ClassFees)
        {
            int LicenseClassID = -1;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"Insert Into LicenseClasses 
                                     (
                                      ClassName,ClassDescription,MinimumAllowedAge, 
                                      DefaultValidityLength,ClassFees)
                                                      Values ( 
                                      @ClassName,@ClassDescription,@MinimumAllowedAge, 
                                      @DefaultValidityLength,@ClassFees);
                                                      SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.Add("@ClassName", SqlDbType.NChar).Value = ClassName;
                        command.Parameters.Add("@ClassDescription", SqlDbType.NChar).Value = ClassDescription;
                        command.Parameters.Add("@MinimumAllowedAge", SqlDbType.TinyInt).Value = MinimumAllowedAge;
                        command.Parameters.Add("@DefaultValidityLength", SqlDbType.TinyInt).Value = DefaultValidityLength;
                        command.Parameters.Add("@ClassFees", SqlDbType.Real).Value = ClassFees;

                        await connection.OpenAsync().ConfigureAwait(false);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow | CommandBehavior.SequentialAccess).ConfigureAwait(false))
                        {

                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                LicenseClassID = Convert.ToInt32(reader.GetValue(0));
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                clsLogger.LogException(ex, $"faild to add new license class with name = {ClassName}");
            }


            return LicenseClassID;

        }

        public static async Task<bool> UpdateLicenseClassAsync(int LicenseClassID, string ClassName,
            string ClassDescription,
            byte MinimumAllowedAge, byte DefaultValidityLength, float ClassFees)
        {

            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {

                    string query = @"Update  LicenseClasses  
                    set ClassName = @ClassName,
                        ClassDescription = @ClassDescription,
                        MinimumAllowedAge = @MinimumAllowedAge,
                        DefaultValidityLength = @DefaultValidityLength,
                        ClassFees = @ClassFees
                        where LicenseClassID = @LicenseClassID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;
                        command.Parameters.Add("@ClassName", SqlDbType.NChar, 50).Value = ClassName;
                        command.Parameters.Add("@ClassDescription", SqlDbType.NChar, 500).Value = ClassDescription;
                        command.Parameters.Add("@MinimumAllowedAge", SqlDbType.TinyInt).Value = MinimumAllowedAge;
                        command.Parameters.Add("@DefaultValidityLength", SqlDbType.TinyInt).Value = DefaultValidityLength;
                        command.Parameters.Add("@ClassFees", SqlDbType.Real).Value = ClassFees;


                        await connection.OpenAsync().ConfigureAwait(false);
                        rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"faild to update new license class with ID = {LicenseClassID}");
            }

            return (rowsAffected > 0);
        }

    }
}
