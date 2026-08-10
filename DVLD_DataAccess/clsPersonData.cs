using System;
using System.Data;
using System.Data.SqlClient;
using DVLD_Shared;
using DVLD_Infrastructure.Storage;

namespace DVLD_DataAccess
{
    public class clsPersonData
    {
        public static bool GetPersonInfoByID(int? PersonID, ref string NationalNo, ref string FirstName, ref string SecondName,
            ref string ThirdName, ref string LastName, ref DateTime DateOfBirth,
            ref short Gender, ref string Address, ref string Phone, ref string Email,
            ref int NationalityCountryID, ref string ImagePath)
        {
            bool IsFound = false;

            if (PersonID == null)
                return false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "SELECT NationalNo, FirstName, SecondName, ThirdName, LastName, DateOfBirth, Gendor, Address, Phone, Email, NationalityCountryID, ImagePath FROM People WHERE PersonID = @PersonID;";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
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

            return IsFound;
        }

        public static bool GetPersonInfoByNationalNo(string NationalNo, ref int? PersonID, ref string FirstName, ref string SecondName,
            ref string ThirdName, ref string LastName, ref DateTime DateOfBirth,
            ref short Gender, ref string Address, ref string Phone, ref string Email,
            ref int NationalityCountryID, ref string ImagePath)
        {
            bool IsFound = false;
            if (string.IsNullOrWhiteSpace(NationalNo))
                return false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "SELECT PersonID, FirstName, SecondName, ThirdName, LastName, DateOfBirth, Gendor, Address, Phone, Email, NationalityCountryID, ImagePath FROM People WHERE NationalNo = @NationalNo;";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@NationalNo", SqlDbType.NVarChar).Value = NationalNo;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                PersonID = reader.GetInt32(0);
                                FirstName = reader.GetString(1);
                                SecondName = reader.GetString(2);
                                ThirdName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                LastName = reader.GetString(4);
                                DateOfBirth = reader.GetDateTime(5);
                                Gender = reader.GetInt16(6);
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

            return IsFound;
        }

        public static Nullable<int> AddNewPerson(string FirstName, string SecondName,
             string ThirdName, string LastName, string NationalNo, DateTime DateOfBirth,
             short Gender, string Address, string Phone, string Email,
             int NationalityCountryID, string ImagePath)
        {
            int? PersonID = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "INSERT INTO People (NationalNo, FirstName, SecondName, ThirdName, " +
                        "LastName, DateOfBirth, Gendor, Address, Phone, Email, NationalityCountryID," +
                        " ImagePath) VALUES (@NationalNo, @FirstName, @SecondName, @ThirdName, @LastName," +
                        " @DateOfBirth, @Gendor, @Address, @Phone, @Email, @NationalityCountryID, @ImagePath);" +
                        "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@NationalNo", SqlDbType.NVarChar).Value = NationalNo;
                        command.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = FirstName;
                        command.Parameters.Add("@SecondName", SqlDbType.NVarChar).Value = SecondName;
                        command.Parameters.Add("@ThirdName", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(ThirdName) ? (object)DBNull.Value : ThirdName;
                        command.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = LastName;
                        command.Parameters.Add("@DateOfBirth", SqlDbType.DateTime).Value = DateOfBirth;
                        command.Parameters.Add("@Gendor", SqlDbType.SmallInt).Value = Gender;
                        command.Parameters.Add("@Address", SqlDbType.NVarChar).Value = Address;
                        command.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = Phone;
                        command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(Email) ? (object)DBNull.Value : Email;
                        command.Parameters.Add("@NationalityCountryID", SqlDbType.Int).Value = NationalityCountryID;
                        command.Parameters.Add("@ImagePath", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(ImagePath) ? (object)DBNull.Value : ImagePath;

                        connection.Open();
                        if (command.ExecuteScalar() is decimal newID)
                        {
                            PersonID = (int)newID;
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

        public static bool UpdatePerson(int? PersonID, string NationalNo, string FirstName, string SecondName,
            string ThirdName, string LastName, DateTime DateOfBirth,
            short Gender, string Address, string Phone, string Email,
            int NationalityCountryID, string ImagePath)
        {
            int NumberOfEffectedRows = 0;

            if (PersonID == null)
                return false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "UPDATE People SET [NationalNo] = @NationalNo, [FirstName] = @FirstName, " +
                        "[SecondName] = @SecondName,[ThirdName] = @ThirdName,[LastName] = @LastName," +
                        "[DateOfBirth] = @DateOfBirth,[Gendor] = @Gendor,[Address] = @Address,[Phone] = @Phone," +
                        "[Email] = @Email,[NationalityCountryID] = @NationalityCountryID,[ImagePath] = @ImagePath" +
                        " WHERE [PersonID] = @PersonID";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        Command.Parameters.Add("@NationalNo", SqlDbType.NVarChar).Value = NationalNo;
                        Command.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = FirstName;
                        Command.Parameters.Add("@SecondName", SqlDbType.NVarChar).Value = SecondName;
                        Command.Parameters.Add("@ThirdName", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(ThirdName) ? (object)DBNull.Value : ThirdName;
                        Command.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = LastName;
                        Command.Parameters.Add("@DateOfBirth", SqlDbType.DateTime).Value = DateOfBirth;
                        Command.Parameters.Add("@Gendor", SqlDbType.SmallInt).Value = Gender;
                        Command.Parameters.Add("@Address", SqlDbType.NVarChar).Value = Address;
                        Command.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = Phone;
                        Command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(Email) ? (object)DBNull.Value : Email;
                        Command.Parameters.Add("@NationalityCountryID", SqlDbType.Int).Value = NationalityCountryID;
                        Command.Parameters.Add("@ImagePath", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(ImagePath) ? (object)DBNull.Value : ImagePath;
                        Command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        connection.Open();
                        NumberOfEffectedRows = Command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to update person info for PersonID: {PersonID}");
            }

            return NumberOfEffectedRows > 0;
        }

        public static bool IsPersonExists(int? PersonID)
        {
            bool IsExist = false;

            if (PersonID == null)
                return false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "SELECT TOP 1 1 FROM People WHERE PersonID = @PersonID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        connection.Open();
                        IsExist = command.ExecuteScalar() != null;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check if person exists by PersonID: {PersonID}");
            }

            return IsExist;

        }

        public static bool IsPersonExists(string NationalNo)
        {
            bool IsExist = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "SELECT TOP 1 1 FROM People WHERE NationalNo = @NationalNo";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@NationalNo", SqlDbType.NVarChar).Value = NationalNo;

                        connection.Open();
                        IsExist = command.ExecuteScalar() != null;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check if person exists by NationalNo: {NationalNo}");
            }

            return IsExist;

        }

        public static bool IsNationalNoUsed(int? PersonID, string NationalNo)
        {
            bool IsUsed = false;

            if (PersonID == null || string.IsNullOrWhiteSpace(NationalNo))
                return false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = @"SELECT TOP 1 1 FROM People
                 WHERE NationalNo = @NationalNo AND PersonID != @PersonID;";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@NationalNo", SqlDbType.NVarChar).Value = NationalNo;
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        connection.Open();
                        IsUsed = command.ExecuteScalar() != null;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check if NationalNo is used for NationalNo: {NationalNo}, PersonID: {PersonID}");
            }

            return IsUsed;

        }


        public static bool DeletePerson(int? PersonID)
        {
            int NumberOfEffectedRows = 0;

            if (PersonID == null)
                return false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "DELETE FROM People WHERE PersonID = @PersonID";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        Command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        connection.Open();
                        NumberOfEffectedRows = Command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to delete person with PersonID: {PersonID}");
            }

            return NumberOfEffectedRows > 0;
        }

        public static DataTable GetAllPeople()
        {
            DataTable Table = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
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
                        connection.Open();
                        using (SqlDataReader reader = Command.ExecuteReader())
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
                clsLogger.LogException(ex, "Failed to retrieve all people.");
            }

            return Table;

        }

        public static bool HasPeople()
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT TOP 1 1 FROM People;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        isFound = command.ExecuteScalar() != null;
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
