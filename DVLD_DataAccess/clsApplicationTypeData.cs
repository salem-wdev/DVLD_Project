using System;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DVLD_Infrastructure.Storage;

namespace DVLD_DataAccess
{
    public class clsApplicationTypeData
    {

        public static async Task<(bool IsFound, string ApplicationTypeTitle, decimal ApplicationFees)> GetApplicationTypeInfoByIDAsync(int ApplicationTypeID)
        {
            bool IsFound = false;
            string ApplicationTypeTitle = string.Empty;
            decimal ApplicationFees = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = "SELECT * FROM ApplicationTypes" +
                    "  WHERE ApplicationTypeID = @ApplicationTypeID;";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);


                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
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

            return (IsFound, ApplicationTypeTitle, ApplicationFees);
        }


        public static async Task<int> AddNewApplicationTypeAsync(string ApplicationTypeTitle,
            decimal ApplicationFees)
        {

            int ApplicationTypeID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {

                    string Query = "INSERT INTO ApplicationTypes " +
                    "(ApplicationTypeTitle, ApplicationFees)" +
                    " VALUES (@ApplicationTypeTitle, @ApplicationFees)" +
                        "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);
                        command.Parameters.AddWithValue("@ApplicationFees", ApplicationFees);

                        await connection.OpenAsync().ConfigureAwait(false);
                        object newApplicationTypeID = await command.ExecuteScalarAsync().ConfigureAwait(false);
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

        public static async Task<bool> UpdateApplicationTypeAsync(int ApplicationTypeID,
        string ApplicationTypeTitle, decimal ApplicationFees)
        {
            int NumberOfEffectedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        NumberOfEffectedRows = await Command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return NumberOfEffectedRows > 0;
        }

        public static async Task<DataTable> GetAllApplicationTypesAsync()
        {
            DataTable Table = new DataTable();

            Table.Columns.Add("ApplicationTypeID", typeof(int));
            Table.Columns.Add("ApplicationTypeTitle", typeof(string));
            Table.Columns.Add("ApplicationFees", typeof(decimal));

            try
            {

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = "SELECT ApplicationTypeID, ApplicationTypeTitle," +
                        " ApplicationFees" +
                        " FROM ApplicationTypes";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await Command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                Table.Rows.Add(reader.GetInt32(0), reader.GetString(1), reader.GetDecimal(2));
                            }
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
