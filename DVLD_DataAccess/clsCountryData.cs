using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public class clsCountryData
    {

        public static bool GetCountryByID(int CountryID, ref string CountryName)
        {
            bool IsFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "SELECT * FROM Countries WHERE CountryID = @CountryID";
                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        Command.Parameters.AddWithValue("@CountryID", CountryID);

                        connection.Open();
                        using (SqlDataReader reader = Command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                CountryName = reader["CountryName"].ToString();
                                IsFound = true;
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

        public static bool GetCountryByCountryName(string CountryName, ref int CountryID)
        {
            bool IsFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "SELECT * FROM Countries WHERE CountryName = @CountryName";
                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        Command.Parameters.AddWithValue("@CountryName", CountryName);

                        connection.Open();
                        using (SqlDataReader reader = Command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                CountryID = (int)reader["CountryID"];
                                IsFound = true;
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


        public static DataTable GetAllCountries()
        {
            DataTable Table = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "SELECT * FROM Countries";

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
            catch (Exception)
            {
            }
            return Table;
        }


    }
}
