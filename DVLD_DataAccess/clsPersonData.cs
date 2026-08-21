using DVLD_Infrastructure.Storage;
using DVLD_Shared;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsPersonData
    {
        private static readonly string _connectionString =
    ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        public static async Task<(bool IsFound, string NationalNo, string FirstName, string SecondName,string ThirdName,
            string LastName, DateTime DateOfBirth,short Gender, string Address, string Phone, string Email,
            int NationalityCountryID, string ImagePath)> GetPersonInfoByIDAsync(int? PersonID)
        {
            bool IsFound = false;
            string NationalNo = string.Empty, FirstName = string.Empty, SecondName = string.Empty,
                ThirdName = string.Empty, LastName = string.Empty, Address = string.Empty,
                Phone = string.Empty, Email = string.Empty, ImagePath = string.Empty;
            DateTime DateOfBirth = DateTime.MinValue;
            short Gender = -1;
            int NationalityCountryID = -1;

            if (PersonID == null)
                return (IsFound, NationalNo, FirstName, SecondName, ThirdName, LastName,
                DateOfBirth, Gender, Address, Phone, Email, NationalityCountryID, ImagePath);
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string Query = "SELECT NationalNo, FirstName, SecondName, ThirdName, LastName, DateOfBirth, Gendor, Address, Phone, Email, NationalityCountryID, ImagePath FROM People WHERE PersonID = @PersonID;";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {

                                NationalNo = reader.GetString(0);
                                FirstName = reader.GetString(1);
                                SecondName = reader.GetString(2);
                                ThirdName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                LastName = reader.GetString(4);
                                DateOfBirth = reader.GetDateTime(5);
                                Gender = reader.GetByte(6);
                                Address = reader.GetString(7);
                                Phone = reader.GetString(8);
                                Email = reader.IsDBNull(9) ? string.Empty : reader.GetString(9);
                                NationalityCountryID = reader.GetInt32(10);
                                ImagePath = reader.IsDBNull(11) ? string.Empty : reader.GetString(11);

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
                clsLogger.LogException(ex, $"Failed to retrieve person info for PersonID: {PersonID ?? 0}");
                IsFound = false;
            }

            return (IsFound, NationalNo, FirstName, SecondName, ThirdName, LastName,
                DateOfBirth, Gender, Address, Phone, Email, NationalityCountryID, ImagePath);
        }

        public static async Task<(bool IsFound, int? PersonID, string FirstName, string SecondName, string ThirdName,
            string LastName, DateTime DateOfBirth, short Gender, string Address, string Phone, string Email,
            int NationalityCountryID, string ImagePath)> GetPersonInfoByNationalNoAsync(string NationalNo)
        {
            bool IsFound = false;
            int? PersonID = null;
            string FirstName = string.Empty, SecondName = string.Empty,
                ThirdName = string.Empty, LastName = string.Empty, Address = string.Empty,
                Phone = string.Empty, Email = string.Empty, ImagePath = string.Empty;
            DateTime DateOfBirth = DateTime.MinValue;
            short Gender = -1;
            int NationalityCountryID = -1;

            if (string.IsNullOrWhiteSpace(NationalNo))
                return (IsFound, PersonID, FirstName, SecondName, ThirdName, LastName,
                DateOfBirth, Gender, Address, Phone, Email, NationalityCountryID, ImagePath);
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string Query = "SELECT PersonID, FirstName, SecondName, ThirdName, LastName, DateOfBirth, Gendor, Address, Phone, Email, NationalityCountryID, ImagePath FROM People WHERE NationalNo = @NationalNo;";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@NationalNo", SqlDbType.NVarChar).Value = NationalNo;

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                PersonID = reader.GetInt32(0);
                                FirstName = reader.GetString(1);
                                SecondName = reader.GetString(2);
                                ThirdName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                LastName = reader.GetString(4);
                                DateOfBirth = reader.GetDateTime(5);
                                Gender = reader.GetByte(6);
                                Address = reader.GetString(7);
                                Phone = reader.GetString(8);
                                Email = reader.IsDBNull(9) ? string.Empty : reader.GetString(9);
                                NationalityCountryID = reader.GetInt32(10);
                                ImagePath = reader.IsDBNull(11) ? string.Empty : reader.GetString(11);

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
                clsLogger.LogException(ex, $"Failed to retrieve person info for NationalNo: {NationalNo}");
                IsFound = false;
            }

            return (IsFound, PersonID, FirstName, SecondName, ThirdName, LastName,
                DateOfBirth, Gender, Address, Phone, Email, NationalityCountryID, ImagePath);
        }

        public static async Task<Nullable<int>> AddNewPersonAsync(string FirstName, string SecondName,
             string ThirdName, string LastName, string NationalNo, DateTime DateOfBirth,
             short Gender, string Address, string Phone, string Email,
             int NationalityCountryID, string ImagePath)
        {
            int? PersonID = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    using (SqlCommand command = new SqlCommand("SP_AddNewPerson", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@NationalNo", SqlDbType.NVarChar, 20).Value = NationalNo;
                        command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 20).Value = FirstName;
                        command.Parameters.Add("@SecondName", SqlDbType.NVarChar, 20).Value = SecondName;
                        command.Parameters.Add("@ThirdName", SqlDbType.NVarChar, 20).Value = (object)ThirdName ?? DBNull.Value;
                        command.Parameters.Add("@LastName", SqlDbType.NVarChar, 20).Value = LastName;
                        command.Parameters.Add("@DateOfBirth", SqlDbType.DateTime).Value = DateOfBirth;
                        command.Parameters.Add("@Gendor", SqlDbType.TinyInt).Value = (byte)Gender;
                        command.Parameters.Add("@Address", SqlDbType.NVarChar, 500).Value = Address;
                        command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = Phone;
                        command.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = (object)Email ?? DBNull.Value;
                        command.Parameters.Add("@NationalityCountryID", SqlDbType.Int).Value = NationalityCountryID;
                        command.Parameters.Add("@ImagePath", SqlDbType.NVarChar, 250).Value = (object)ImagePath ?? DBNull.Value;

                        SqlParameter outputIdParam = new SqlParameter("@NewPersonID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        command.Parameters.Add(outputIdParam);

                        await connection.OpenAsync().ConfigureAwait(false);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        if (outputIdParam.Value != null && outputIdParam.Value != DBNull.Value)
                        {
                            PersonID = (int?)outputIdParam.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to add new person with NationalNo: {NationalNo}");
            }

            return PersonID;
        }

        public static async Task<bool> UpdatePersonAsync(int? PersonID, string NationalNo, string FirstName, string SecondName,
            string ThirdName, string LastName, DateTime DateOfBirth, short Gender, string Address, string Phone, string Email,
            int NationalityCountryID, string ImagePath)
        {
            int NumberOfEffectedRows = 0;

            if (PersonID == null)
                return false;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = "UPDATE People SET [NationalNo] = @NationalNo, [FirstName] = @FirstName, " +
                        "[SecondName] = @SecondName,[ThirdName] = @ThirdName,[LastName] = @LastName," +
                        "[DateOfBirth] = @DateOfBirth,[Gendor] = @Gendor,[Address] = @Address,[Phone] = @Phone," +
                        "[Email] = @Email,[NationalityCountryID] = @NationalityCountryID,[ImagePath] = @ImagePath" +
                        " WHERE [PersonID] = @PersonID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@NationalNo", SqlDbType.NVarChar, 20).Value = NationalNo;
                        command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 20).Value = FirstName;
                        command.Parameters.Add("@SecondName", SqlDbType.NVarChar, 20).Value = SecondName;
                        command.Parameters.Add("@ThirdName", SqlDbType.NVarChar, 20).Value = (object)ThirdName ?? DBNull.Value;
                        command.Parameters.Add("@LastName", SqlDbType.NVarChar, 20).Value = LastName;
                        command.Parameters.Add("@DateOfBirth", SqlDbType.DateTime).Value = DateOfBirth;
                        command.Parameters.Add("@Gendor", SqlDbType.TinyInt).Value = (byte)Gender;
                        command.Parameters.Add("@Address", SqlDbType.NVarChar, 500).Value = Address;
                        command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = Phone;
                        command.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = (object)Email ?? DBNull.Value;
                        command.Parameters.Add("@NationalityCountryID", SqlDbType.Int).Value = NationalityCountryID;
                        command.Parameters.Add("@ImagePath", SqlDbType.NVarChar, 250).Value = (object)ImagePath ?? DBNull.Value; 
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        await connection.OpenAsync().ConfigureAwait(false);
                        NumberOfEffectedRows = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to update person info for PersonID: {PersonID}");
            }

            return NumberOfEffectedRows > 0;
        }

        public static async Task<bool> IsPersonExistsAsync(int? PersonID)
        {
            bool IsExist = false;

            if (PersonID == null)
                return false;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = "SELECT TOP 1 1 FROM People WHERE PersonID = @PersonID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        await connection.OpenAsync().ConfigureAwait(false);
                        IsExist = await command.ExecuteScalarAsync().ConfigureAwait(false) != null;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check if person exists by PersonID: {PersonID}");
            }

            return IsExist;

        }

        public static async Task<bool> IsPersonExistsAsync(string NationalNo)
        {
            bool IsExist = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = "SELECT TOP 1 1 FROM People WHERE NationalNo = @NationalNo";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@NationalNo", SqlDbType.NVarChar).Value = NationalNo;

                        await connection.OpenAsync().ConfigureAwait(false);
                        IsExist = await command.ExecuteScalarAsync().ConfigureAwait(false) != null;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check if person exists by NationalNo: {NationalNo}");
            }

            return IsExist;

        }

        public static async Task<bool> IsNationalNoUsedAsync(int? PersonID, string NationalNo)
        {
            bool IsUsed = false;

            if (PersonID == null || string.IsNullOrWhiteSpace(NationalNo))
                return false;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"SELECT TOP 1 1 FROM People
                 WHERE NationalNo = @NationalNo AND PersonID != @PersonID;";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@NationalNo", SqlDbType.NVarChar).Value = NationalNo;
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        await connection.OpenAsync().ConfigureAwait(false);
                        IsUsed = await command.ExecuteScalarAsync().ConfigureAwait(false) != null;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check if NationalNo is used for NationalNo: {NationalNo}, PersonID: {PersonID}");
            }

            return IsUsed;

        }

        public static async Task<bool> DeletePersonAsync(int? PersonID)
        {
            int NumberOfEffectedRows = 0;

            if (PersonID == null)
                return false;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = "DELETE FROM People WHERE PersonID = @PersonID";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        Command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        await connection.OpenAsync().ConfigureAwait(false);
                        NumberOfEffectedRows = await Command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to delete person with PersonID: {PersonID}");
            }

            return NumberOfEffectedRows > 0;
        }

        public static async Task<DataTable> GetAllPeopleAsync()
        {
            DataTable Table = new DataTable();

            Table.Columns.Add("PersonID", typeof(int));
            Table.Columns.Add("NationalNo", typeof(string));
            Table.Columns.Add("FirstName", typeof(string));
            Table.Columns.Add("SecondName", typeof(string));
            Table.Columns.Add("ThirdName", typeof(string));
            Table.Columns.Add("LastName", typeof(string));
            Table.Columns.Add("DateOfBirth", typeof(DateTime));
            Table.Columns.Add("GendorCaption", typeof(string));
            Table.Columns.Add("Address", typeof(string));
            Table.Columns.Add("Phone", typeof(string));
            Table.Columns.Add("Email", typeof(string));
            Table.Columns.Add("CountryName", typeof(string));
            Table.Columns.Add("ImagePath", typeof(string));

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = "SELECT People.PersonID, People.NationalNo, People.FirstName, People.SecondName, " +
                        "People.ThirdName, People.LastName, People.DateOfBirth, " +
                        "CASE      " +
                        "WHEN People.Gendor = 0 THEN 'Male'   " +
                        "  ELSE 'Female' END AS GendorCaption, " +
                        "People.Address, People.Phone, People.Email," +
                        "Countries.CountryName, People.ImagePath " +
                        "FROM  People " +
                        "INNER JOIN  " +
                        "Countries  ON People.NationalityCountryID = Countries.CountryID " +
                        "ORDER BY  People.FirstName;";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await Command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                Table.Rows.Add(
                                        reader.GetInt32(0),                                                 // PersonID
                                        reader.GetString(1),                                                // NationalNo
                                        reader.GetString(2),                                                // FirstName
                                        reader.IsDBNull(3) ? (object)DBNull.Value : reader.GetString(3),    // SecondName
                                        reader.IsDBNull(4) ? (object)DBNull.Value : reader.GetString(4),    // ThirdName
                                        reader.GetString(5),                                                // LastName
                                        reader.GetDateTime(6),                                              // DateOfBirth
                                        reader.GetString(7),                                                // GendorCaption
                                        reader.IsDBNull(8) ? (object)DBNull.Value : reader.GetString(8),    // Address
                                        reader.IsDBNull(9) ? (object)DBNull.Value : reader.GetString(9),    // Phone
                                        reader.IsDBNull(10) ? (object)DBNull.Value : reader.GetString(10),  // Email
                                        reader.GetString(11),                                               // CountryName
                                        reader.IsDBNull(12) ? (object)DBNull.Value : reader.GetString(12)   // ImagePath
                                    );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Failed to retrieve all people.");
            }

            return Table;

        }

        public static async Task<bool> HasPeopleAsync()
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = "SELECT TOP 1 1 FROM People;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync().ConfigureAwait(false);
                        isFound = await command.ExecuteScalarAsync().ConfigureAwait(false) != null;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Failed to check if there are people.");
                isFound = false;
            }

            return isFound;
        }


    }
}
