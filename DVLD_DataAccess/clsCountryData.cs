using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DVLD_Infrastructure.Storage;

namespace DVLD_DataAccess
{
    public class clsCountryData
    {
        private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        public static async Task<string> GetCountryByIDAsync(int CountryID)
        {
            string CountryName = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string Query = "SELECT CountryName FROM Countries WHERE CountryID = @CountryID";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        Command.Parameters.Add("@CountryID", SqlDbType.Int).Value = CountryID;

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await Command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                CountryName = reader.GetString(0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred in GetCountryByIDAsync method for CountryID {CountryID}");
            }
            return CountryName;
        }

        public static async Task<int?> GetCountryByCountryNameAsync(string CountryName)
        {
            int? CountryID = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string Query = "SELECT CountryID FROM Countries WHERE CountryName = @CountryName";
                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        Command.Parameters.Add("@CountryName", SqlDbType.NVarChar, 50).Value = CountryName;

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await Command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                                CountryID = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred in GetCountryByCountryName method for CountryName: {CountryName}");
            }
            return CountryID;
        }

        public static async Task<DataTable> GetAllCountriesAsync()
        {
            DataTable Table = new DataTable();
            Table.Columns.Add("CountryID", typeof(int));
            Table.Columns.Add("CountryName", typeof(string));
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string Query = "SELECT CountryID, CountryName FROM Countries";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await Command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                                Table.Rows.Add(reader.GetInt32(0), reader.GetString(1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred in GetAllCountries method");
            }
            return Table;
        }
    }
}
