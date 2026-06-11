using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsUserData
    {
        public static bool GetUserInfoByUserID(int UserID, ref int PersonID, ref string UserName, 
            ref string Password, ref bool IsActive)
        {
            bool IsFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT[UserID] " +
                ",[PersonID] " +
                ",[UserName] " +
                ",[Password] " +
                ",[IsActive] " +
                "FROM Users  " +
                " WHERE UserID = @UserID"
;

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@UserID", UserID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    PersonID = Convert.ToInt32(reader["PersonID"]);
                    UserName = reader["UserName"].ToString();
                    Password = reader["Password"].ToString();
                    IsActive = Convert.ToBoolean(reader["IsActive"]);

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

        public static bool GetUserInfoByUserName(string UserName,
           ref int UserID, ref int PersonID,  
            ref string Password, ref bool IsActive)
        {
            bool IsFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT[UserID] " +
                ",[PersonID] " +
                ",[UserName] " +
                ",[Password] " +
                ",[IsActive] " +
                "FROM Users  " +
                " WHERE UserName = @UserName"
;

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@UserName", UserName);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    PersonID = Convert.ToInt32(reader["PersonID"]);
                    UserID = Convert.ToInt32(reader["UserID"]);
                    Password = reader["Password"].ToString();
                    IsActive = Convert.ToBoolean(reader["IsActive"]);

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

        public static bool GetUserInfoByPersonID(int PersonID,
            ref int UserID, ref string UserName, 
            ref string Password, ref bool IsActive)
        {
            bool IsFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT[UserID] " +
                ",[PersonID] " +
                ",[UserName] " +
                ",[Password] " +
                ",[IsActive] " +
                "FROM Users  " +
                " WHERE PersonID = @PersonID"
;

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    UserID = Convert.ToInt32(reader["UserID"]);
                    UserName = reader["UserName"].ToString();
                    Password = reader["Password"].ToString();
                    IsActive = Convert.ToBoolean(reader["IsActive"]);

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

        public static bool GetUserInfoByUsernameAndPassword(string UserName, string Password,
    ref int UserID, ref int PersonID, ref bool IsActive)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = "SELECT * FROM Users WHERE Username = @Username and Password=@Password;";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Username", UserName);
            command.Parameters.AddWithValue("@Password", Password);


            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    isFound = true;
                    UserID = (int)reader["UserID"];
                    PersonID = (int)reader["PersonID"];
                    UserName = (string)reader["UserName"];
                    Password = (string)reader["Password"];
                    IsActive = (bool)reader["IsActive"];


                }
                else
                {
                    // The record was not found
                    isFound = false;
                }

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


        public static int AddNewUser(int PersonID, string UserName,
            string Password, bool IsActive)
        {

            int UserID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "INSERT INTO [dbo].[Users] ([PersonID], [UserName], [Password], [IsActive])" +
                " VALUES (@PersonID, @UserName, @Password, @IsActive);" +
                "SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);
            command.Parameters.AddWithValue("@UserName", UserName);
            command.Parameters.AddWithValue("@Password", Password);
            command.Parameters.AddWithValue("@IsActive", IsActive);

            try
            {
                connection.Open();
                object newUserID = command.ExecuteScalar();
                if (int.TryParse(newUserID.ToString(), out int NewID))
                {
                    UserID = NewID;
                }
                else
                {
                    UserID = -1;
                }

            }
            catch { }
            finally
            {
                connection.Close();
            }

            return UserID;

        }

        public static bool UpdateUser(int UserID, int PersonID, string UserName,
            string Password, bool IsActive)
        {
            int NumberOfEffectedRows = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "UPDATE Users SET [PersonID] = @PersonID, " +
                "[UserName] = @UserName, [Password] = @Password," +
                " [IsActive] = @IsActive WHERE [UserID] = @UserID";

            SqlCommand Command = new SqlCommand(Query, connection);

            Command.Parameters.AddWithValue("@PersonID", PersonID);
            Command.Parameters.AddWithValue("@UserName", UserName);
            Command.Parameters.AddWithValue("@Password", Password);
            Command.Parameters.AddWithValue("@IsActive", IsActive);
            Command.Parameters.AddWithValue("@UserID", UserID);

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

        public static bool IsUserExists(int UserID)
        {
            bool IsExist = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT Found=1  FROM Users" +
                " WHERE UserID = @UserID";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@UserID", UserID);

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

        public static bool IsUserExists(string UserName)
        {
            bool IsExist = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT Found=1 FROM Users" +
                " WHERE UserName = @UserName";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@UserName", UserName);

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

        public static bool IsUserExistForPersonID(int PersonID)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = "SELECT Found=1 FROM Users WHERE PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", PersonID);

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

        public static bool DeleteUser(int UserID)
        {
            int NumberOfEffectedRows = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "DELETE FROM Users " +
                "WHERE UserID = @UserID";

            SqlCommand Command = new SqlCommand(Query, connection);

            Command.Parameters.AddWithValue("@UserID", UserID);

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

        public static DataTable GetAllUsers()
        {
            DataTable Table = new DataTable();


            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"SELECT  Users.UserID, Users.PersonID,
                            FullName = People.FirstName + ' ' + People.SecondName + ' ' + ISNULL( People.ThirdName,'') +' ' + People.LastName,
                             Users.UserName, Users.IsActive
                             FROM  Users INNER JOIN
                                    People ON Users.PersonID = People.PersonID";

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

        public static bool DoesPersonHaveUser44(int PersonID)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = "SELECT Found=1 FROM Users WHERE PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", PersonID);

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

        public static bool ChangeUserCredentials(int UserID, string NewUserName, string NewPassword)
        {

            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Update  Users  
                            set UserName = @UserName,
                                Password = @Password
                            where UserID = @UserID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@UserName", NewUserName);
            command.Parameters.AddWithValue("@Password", NewPassword);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                return false;
            }

            finally
            {
                connection.Close();
            }

            return (rowsAffected > 0);
        }

        public static bool ChangePassword(int UserID, string NewPassword)
        {

            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Update  Users  
                            set Password = @Password
                            where UserID = @UserID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@Password", NewPassword);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                return false;
            }

            finally
            {
                connection.Close();
            }

            return (rowsAffected > 0);
        }

        public static bool ChangeUserActivity(int UserID, bool IsActive)
        {
            bool IsSucceed = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"UPDATE [dbo].[Users]
                               SET 
                                  [IsActive] = @IsActive
                             WHERE UserID = @UserID";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@IsActive", IsActive);

            try
            {
                connection.Open();
                IsSucceed = (command.ExecuteNonQuery() > 0);
            }
            catch { }
            finally
            {
                connection.Close();
            }

            return IsSucceed;

        }

    }
}
