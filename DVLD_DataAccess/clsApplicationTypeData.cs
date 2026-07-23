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
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "SELECT * FROM ApplicationTypes" +
                    "  WHERE ApplicationTypeID = @ApplicationTypeID;";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);


                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                ApplicationTypeTitle = reader["ApplicationTypeTitle"].ToString();
                                ApplicationFees = Convert.ToDecimal(reader["ApplicationFees"]);

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
            }

            return IsFound;
        }


        public static int AddNewApplicationType(string ApplicationTypeTitle,
            decimal ApplicationFees)
        {

            int ApplicationTypeID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string Query = "INSERT INTO ApplicationTypes " +
                    "(ApplicationTypeTitle, ApplicationFees)" +
                    " VALUES (@ApplicationTypeTitle, @ApplicationFees)" +
                        "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);
                        command.Parameters.AddWithValue("@ApplicationFees", ApplicationFees);

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
                }
            }
            catch (Exception ex)
            {
            }
            return ApplicationTypeID;
        }

        public static bool UpdateApplicationType(int ApplicationTypeID,
        string ApplicationTypeTitle, decimal ApplicationFees)
        {
            int NumberOfEffectedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string Query = "UPDATE ApplicationTypes " +
                        "SET ApplicationTypeTitle = @ApplicationTypeTitle " +
                        ",ApplicationFees = @ApplicationFees" +
                        " WHERE ApplicationTypeID = @ApplicationTypeID";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);
                        Command.Parameters.AddWithValue("@ApplicationFees", ApplicationFees);
                        Command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

                        connection.Open();
                        NumberOfEffectedRows = Command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return NumberOfEffectedRows > 0;
        }

        public static DataTable GetAllApplicationTypes()
        {
            DataTable Table = new DataTable();

            try
            {

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "SELECT ApplicationTypeID, ApplicationTypeTitle," +
                        " ApplicationFees" +
                        " FROM ApplicationTypes";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = Command.ExecuteReader())
                        {
                            Table.Load(reader);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
            }
            return Table;

        }

    }


}
