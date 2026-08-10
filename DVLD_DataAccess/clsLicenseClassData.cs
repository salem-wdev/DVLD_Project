using DVLD_Shared;
using DVLD_Infrastructure.Storage;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public class clsLicenseClassData
    {
        public static bool GetLicenseClassInfoByID(int LicenseClassID,
    ref string ClassName, ref string ClassDescription, ref byte MinimumAllowedAge,
    ref byte DefaultValidityLength, ref float ClassFees)
        {
            bool isFound = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string query = "SELECT * FROM LicenseClasses WHERE LicenseClassID = @LicenseClassID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.Read())
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

            return isFound;
        }


        public static bool GetLicenseClassInfoByClassName(string ClassName, ref int LicenseClassID,
            ref string ClassDescription, ref byte MinimumAllowedAge,
           ref byte DefaultValidityLength, ref float ClassFees)
        {
            bool isFound = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string query = "SELECT * FROM LicenseClasses WHERE ClassName = @ClassName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ClassName", ClassName);


                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.Read())
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

            return isFound;
        }



        public static DataTable GetAllLicenseClasses()
        {

            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM LicenseClasses order by ClassName";

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

                clsLogger.LogException(ex, $"faild to get all license classes Table");
            }

            return dt;

        }

        public static int AddNewLicenseClass(string ClassName, string ClassDescription,
            byte MinimumAllowedAge, byte DefaultValidityLength, float ClassFees)
        {
            int LicenseClassID = -1;
            try
            {

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
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

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SequentialAccess))
                        {

                            if (reader.Read())
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

        public static bool UpdateLicenseClass(int LicenseClassID, string ClassName,
            string ClassDescription,
            byte MinimumAllowedAge, byte DefaultValidityLength, float ClassFees)
        {

            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
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


                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

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
