using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public class clsPersonData
    {
        public static bool GetPersonInfoByID(int PersonID, ref string NationalNo, ref string FirstName, ref string SecondName,
            ref string ThirdName, ref string LastName, ref DateTime DateOfBirth,
            ref short Gender, ref string Address, ref string Phone, ref string Email,
            ref int NationalityCountryID, ref string ImagePath)
        {
            bool IsFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT * FROM People  WHERE PersonID = @PersonID;";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {

                    NationalNo = reader["NationalNo"].ToString();
                    FirstName = reader["FirstName"].ToString();
                    SecondName = reader["SecondName"].ToString();

                    //ThirdName: allows null, we should handle null value
                    if (reader["ThirdName"] == DBNull.Value)
                    {
                        ThirdName = string.Empty;
                    }
                    else
                    {
                        ThirdName = reader["ThirdName"].ToString();
                    }

                    LastName = reader["LastName"].ToString();
                    DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]);
                    Gender = Convert.ToInt16(reader["Gendor"]);
                    Address = reader["Address"].ToString();
                    Phone = reader["Phone"].ToString();

                    //Email: allows null, we should handle null value
                    if (reader["Email"] == DBNull.Value)
                    {
                        Email = string.Empty;
                    }
                    else
                    {
                        Email = reader["Email"].ToString();
                    }

                    NationalityCountryID = Convert.ToInt32(reader["NationalityCountryID"]);

                    //ImagePath: allows null, we should handle null value
                    if (reader["ImagePath"].ToString() == string.Empty)
                    {
                        ImagePath = string.Empty;
                    }
                    else
                    {
                        ImagePath = reader["ImagePath"].ToString();
                    }

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

        public static bool GetPersonInfoByNationalNo(string NationalNo, ref int PersonID, ref string FirstName, ref string SecondName,
            ref string ThirdName, ref string LastName, ref DateTime DateOfBirth,
            ref short Gender, ref string Address, ref string Phone, ref string Email,
            ref int NationalityCountryID, ref string ImagePath)
        {
            bool IsFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT * FROM People  WHERE NationalNo = @NationalNo;";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@NationalNo", NationalNo);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    PersonID = Convert.ToInt32(reader["PersonID"]);
                    FirstName = reader["FirstName"].ToString();
                    SecondName = reader["SecondName"].ToString();

                    //ThirdName: allows null, we should handle null value
                    if (reader["ThirdName"] == DBNull.Value)
                    {
                        ThirdName = string.Empty;
                    }
                    else
                    {
                        ThirdName = reader["ThirdName"].ToString();
                    }

                    LastName = reader["LastName"].ToString();
                    DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]);
                    Gender = Convert.ToInt16(reader["Gendor"]);
                    Address = reader["Address"].ToString();
                    Phone = reader["Phone"].ToString();

                    //Email: allows null, we should handle null value
                    if (reader["Email"] == DBNull.Value)
                    {
                        Email = string.Empty;
                    }
                    else
                    {
                        Email = reader["Email"].ToString();
                    }

                    NationalityCountryID = Convert.ToInt32(reader["NationalityCountryID"]);

                    //ImagePath: allows null, we should handle null value
                    if (reader["ImagePath"].ToString() == string.Empty)
                    {
                        ImagePath = string.Empty;
                    }
                    else
                    {
                        ImagePath = reader["ImagePath"].ToString();
                    }

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

        public static int AddNewPerson(string FirstName, string SecondName,
             string ThirdName, string LastName, string NationalNo, DateTime DateOfBirth,
             short Gender, string Address, string Phone, string Email,
             int NationalityCountryID, string ImagePath)
        {

            int PersonID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "INSERT INTO People (  NationalNo, FirstName, SecondName, ThirdName, " +
                "LastName, DateOfBirth, Gendor, Address, Phone, Email, NationalityCountryID," +
                " ImagePath) VALUES (@NationalNo, @FirstName, @SecondName, @ThirdName, @LastName," +
                " @DateOfBirth, @Gendor, @Address, @Phone, @Email, @NationalityCountryID, @ImagePath);" +
                "SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@NationalNo", NationalNo);
            command.Parameters.AddWithValue("@FirstName", FirstName);
            command.Parameters.AddWithValue("@SecondName", SecondName);

            if (ThirdName == string.Empty && ThirdName == null)
            {
                command.Parameters.AddWithValue("@ThirdName", DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@ThirdName", ThirdName);
            }

            command.Parameters.AddWithValue("@LastName", LastName);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Gendor", Gender);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Phone", Phone);

            if (Email == string.Empty && Email == null)
            {
                command.Parameters.AddWithValue("@Email", DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@Email", Email);
            }

            command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);

            if (string.IsNullOrEmpty(ImagePath))
            {
                command.Parameters.AddWithValue("@ImagePath", DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@ImagePath", ImagePath);
            }

            try
            {
                connection.Open();
                object newPersonID = command.ExecuteScalar();
                if (int.TryParse(newPersonID.ToString(), out int NewID))
                {
                    PersonID = NewID;
                }
                else
                {
                    PersonID = -1;
                }

            }
            catch { }
            finally
            {
                connection.Close();
            }

            return PersonID;

        }

        public static bool UpdatePerson(int PersonID, string NationalNo, string FirstName, string SecondName,
            string ThirdName, string LastName, DateTime DateOfBirth,
            short Gender, string Address, string Phone, string Email,
            int NationalityCountryID, string ImagePath)
        {
            int NumberOfEffectedRows = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "UPDATE People SET [NationalNo] = @NationalNo, [FirstName] = @FirstName, " +
                "[SecondName] = @SecondName,[ThirdName] = @ThirdName,[LastName] = @LastName," +
                "[DateOfBirth] = @DateOfBirth,[Gendor] = @Gendor,[Address] = @Address,[Phone] = @Phone," +
                "[Email] = @Email,[NationalityCountryID] = @NationalityCountryID,[ImagePath] = @ImagePath" +
                " WHERE [PersonID] = @PersonID";

            SqlCommand Command = new SqlCommand(Query, connection);

            Command.Parameters.AddWithValue("@NationalNo", NationalNo);
            Command.Parameters.AddWithValue("@FirstName", FirstName);
            Command.Parameters.AddWithValue("@SecondName", SecondName);

            if (string.IsNullOrEmpty(ThirdName))
            {
                Command.Parameters.AddWithValue("@ThirdName", DBNull.Value);
            }
            else
            {
                Command.Parameters.AddWithValue("@ThirdName", ThirdName);
            }

            Command.Parameters.AddWithValue("@LastName", LastName);
            Command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            Command.Parameters.AddWithValue("@Gendor", Gender);
            Command.Parameters.AddWithValue("@Address", Address);
            Command.Parameters.AddWithValue("@Phone", Phone);

            if (string.IsNullOrEmpty(Email))
            {
                Command.Parameters.AddWithValue("@Email", DBNull.Value);
            }
            else
            {
                Command.Parameters.AddWithValue("@Email", Email);
            }

            Command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);

            if (string.IsNullOrEmpty(ImagePath))
            {
                Command.Parameters.AddWithValue("@ImagePath", DBNull.Value);
            }
            else
            {
                Command.Parameters.AddWithValue("@ImagePath", ImagePath);
            }

            Command.Parameters.AddWithValue("@PersonID", PersonID);

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

        public static bool IsPersonExists(int PersonID)
        {
            bool IsExist = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT Found=1  FROM People" +
                " WHERE PersonID = @PersonID";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    IsExist = true;
                }
                else
                {
                    IsExist = false;
                }
            }
            catch { }
            finally
            {
                connection.Close();
            }

            return IsExist;

        }

        public static bool IsPersonExists(string NationalNo)
        {
            bool IsExist = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT Found=1 FROM People" +
                " WHERE NationalNo = @NationalNo";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@NationalNo", NationalNo);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    IsExist = true;
                }
                else
                {
                    IsExist = false;
                }
            }
            catch { }
            finally
            {
                connection.Close();
            }

            return IsExist;

        }

        public static bool DeletePerson(int PersonID)
        {
            int NumberOfEffectedRows = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "DELETE FROM People " +
                "WHERE PersonID = @PersonID";

            SqlCommand Command = new SqlCommand(Query, connection);

            Command.Parameters.AddWithValue("@PersonID", PersonID);

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

        public static DataTable GetAllPeople()
        {
            DataTable Table = new DataTable();


            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

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

        public static bool HasPeople()
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = "SELECT TOP 1 Found=1 FROM People;";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                isFound = reader.HasRows;

                reader.Close();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }


    }
}
