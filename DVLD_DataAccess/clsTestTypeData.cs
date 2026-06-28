using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public class clsTestTypeData
    {
        public static bool GetTestTypeInfoByID(int TestTypeID,
        ref string TestTypeTitle, ref string TestTypeDescription, ref decimal TestTypeFees)
        {
            bool IsFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT TestTypeID, TestTypeTitle," +
                " TestTypeDescription, TestTypeFees" +
                " FROM TestTypes" +
                " WHERE TestTypeID = @TestTypeID;";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {

                    TestTypeTitle = reader["TestTypeTitle"].ToString();
                    TestTypeDescription = reader["TestTypeDescription"].ToString();
                    TestTypeFees = Convert.ToDecimal(reader["TestTypeFees"]);


                    reader.Close();

                    IsFound = true;
                }
                else
                {
                    IsFound = false;
                }
            }
            catch { }
            finally
            {
                connection.Close();
            }

            return IsFound;
        }


        public static int AddNewTestType(string TestTypeTitle,
           string TestTypeDescription, decimal TestTypeFees)
        {

            int TestTypeID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "INSERT INTO TestTypes " +
            "(TestTypeTitle, TestTypeDescription, TestTypeFees)" +
            " VALUES (@TestTypeTitle, @TestTypeDescription, @TestTypeFees);" +
                " SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@TestTypeTitle", TestTypeTitle);
            command.Parameters.AddWithValue("@TestTypeFees", TestTypeFees);
            command.Parameters.AddWithValue("@TestTypeDescription", TestTypeDescription);

            try
            {
                connection.Open();
                object newTestTypeID = command.ExecuteScalar();
                if (int.TryParse(newTestTypeID?.ToString(), out int NewID))
                {
                    TestTypeID = NewID;
                }
                else
                {
                    TestTypeID = -1;
                }

            }
            catch { }
            finally
            {
                connection.Close();
            }

            return TestTypeID;

        }

        public static bool UpdateTestType(int TestTypeID,
        string TestTypeTitle, string TestTypeDescription, decimal TestTypeFees)
        {
            int NumberOfEffectedRows = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "UPDATE TestTypes " +
                "SET TestTypeTitle = @TestTypeTitle " +
                ",TestTypeDescription = @TestTypeDescription" +
                ",TestTypeFees = @TestTypeFees" +
                " WHERE TestTypeID = @TestTypeID";

            SqlCommand Command = new SqlCommand(Query, connection);

            Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
            Command.Parameters.AddWithValue("@TestTypeDescription", TestTypeDescription);
            Command.Parameters.AddWithValue("@TestTypeTitle", TestTypeTitle);
            Command.Parameters.AddWithValue("@TestTypeFees", TestTypeFees);


            try
            {
                connection.Open();
                NumberOfEffectedRows = Command.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                connection.Close();
            }

            return NumberOfEffectedRows > 0;

        }

        public static DataTable GetAllTestTypes()
        {
            DataTable Table = new DataTable();


            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT TestTypeID, TestTypeTitle," +
                " TestTypeDescription, TestTypeFees" +
                " FROM TestTypes";

            SqlCommand Command = new SqlCommand(Query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = Command.ExecuteReader();

                Table.Load(reader);
                reader.Close();
            }
            catch { }
            finally
            {
                connection.Close();
            }

            return Table;

        }

    }
}
