using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public class clsApplicationTypeData
    {

        public static bool GetApplicationTypeInfoByID(int ApplicationTypeID,
        ref string ApplicationTypeTitle, ref decimal ApplicationFees)
        {
            bool IsFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT * FROM ApplicationTypes" +
            "  WHERE ApplicationTypeID = @ApplicationTypeID;";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {

                    ApplicationTypeTitle = reader["ApplicationTypeTitle"].ToString();
                    ApplicationFees = Convert.ToDecimal(reader["ApplicationFees"]);


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


        public static int AddNewApplicationType(string ApplicationTypeTitle,
            decimal ApplicationFees)
        {

            int ApplicationTypeID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "INSERT INTO ApplicationTypes " +
            "(ApplicationTypeTitle, ApplicationFees)" +
            " VALUES (@ApplicationTypeTitle, @ApplicationFees)" +
                "SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);
            command.Parameters.AddWithValue("@ApplicationFees", ApplicationFees);


            try
            {
                connection.Open();
                object newApplicationTypeID = command.ExecuteScalar();
                if (int.TryParse(newApplicationTypeID.ToString(), out int NewID))
                {
                    ApplicationTypeID = NewID;
                }
                else
                {
                    ApplicationTypeID = -1;
                }

            }
            catch { }
            finally
            {
                connection.Close();
            }

            return ApplicationTypeID;

        }

        public static bool UpdateApplicationType(int ApplicationTypeID,
        string ApplicationTypeTitle, decimal ApplicationFees)
        {
            int NumberOfEffectedRows = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "UPDATE ApplicationTypes " +
                "SET ApplicationTypeTitle = @ApplicationTypeTitle " +
                ",ApplicationFees = @ApplicationFees" +
                " WHERE ApplicationTypeID = @ApplicationTypeID";

            SqlCommand Command = new SqlCommand(Query, connection);

            Command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);
            Command.Parameters.AddWithValue("@ApplicationFees", ApplicationFees);
            Command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);


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

        public static DataTable GetAllApplicationTypes()
        {
            DataTable Table = new DataTable();


            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT ApplicationTypeID, ApplicationTypeTitle," +
                " ApplicationFees" +
                " FROM ApplicationTypes";

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
