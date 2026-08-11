using DVLD_Shared;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DVLD_Infrastructure.Storage;

namespace DVLD_DataAccess
{
    public class clsUserData
    {
        public static bool GetUserInfoByUserID(int UserID, ref int PersonID, ref string UserName,
            ref string Password, ref bool IsActive)
        {
            bool IsFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"SELECT PersonID, UserName, [Password], IsActive
                                     FROM Users
                                     WHERE UserID = @UserID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                PersonID = reader.GetInt32(0);
                                UserName = reader.GetString(1);
                                Password = reader.GetString(2);
                                IsActive = reader.GetBoolean(3);
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
                clsLogger.LogException(ex, $"Failed to retrieve user info by UserID: {UserID}");
                IsFound = false;
            }

            return IsFound;
        }

        public static bool GetUserInfoByUserName(string UserName,
           ref int UserID, ref int PersonID,
            ref string Password, ref bool IsActive)
        {
            bool IsFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"SELECT UserID, PersonID, [Password], IsActive
                                     FROM Users
                                     WHERE UserName = @UserName";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = UserName;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                UserID = reader.GetInt32(0);
                                PersonID = reader.GetInt32(1);
                                Password = reader.GetString(2);
                                IsActive = reader.GetBoolean(3);
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
                clsLogger.LogException(ex, $"Failed to retrieve user info by UserName: {UserName}");
                IsFound = false;
            }

            return IsFound;
        }

        public static bool GetUserInfoByPersonID(int PersonID,
            ref int UserID, ref string UserName,
            ref string Password, ref bool IsActive)
        {
            bool IsFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"SELECT UserID, UserName, [Password], IsActive
                                     FROM Users
                                     WHERE PersonID = @PersonID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                UserID = reader.GetInt32(0);
                                UserName = reader.GetString(1);
                                Password = reader.GetString(2);
                                IsActive = reader.GetBoolean(3);
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
                clsLogger.LogException(ex, $"Failed to retrieve user info by PersonID: {PersonID}");
                IsFound = false;
            }

            return IsFound;
        }

        public static bool GetUserInfoByUsernameAndPassword(string UserName, string Password,
    ref int UserID, ref int PersonID, ref bool IsActive)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT UserID, PersonID, UserName, [Password], IsActive
                                     FROM Users
                                     WHERE UserName = @UserName
                                       AND [Password] = @Password;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = UserName;
                        command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = Password;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                UserID = reader.GetInt32(0);
                                PersonID = reader.GetInt32(1);
                                UserName = reader.GetString(2);
                                Password = reader.GetString(3);
                                IsActive = reader.GetBoolean(4);
                            }
                            else
                            {
                                isFound = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve user info by username and password for UserName: {UserName}");
                isFound = false;
            }

            return isFound;
        }


        public static int AddNewUser(int PersonID, string UserName,
            string Password, bool IsActive)
        {
            int UserID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"INSERT INTO [Users] ([PersonID], [UserName], [Password], [IsActive])
                                     VALUES (@PersonID, @UserName, @Password, @IsActive);
                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;
                        command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = UserName;
                        command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = Password;
                        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = IsActive;

                        connection.Open();
                        if (command.ExecuteScalar() is int NewID)
                        {
                            UserID = NewID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to add new user for PersonID: {PersonID}");
            }

            return UserID;
        }

        public static bool UpdateUser(int UserID, string UserName,
            string Password, bool IsActive)
        {
            int NumberOfEffectedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"UPDATE Users
                                     SET [UserName] = @UserName,
                                         [Password] = @Password,
                                         [IsActive] = @IsActive
                                     WHERE [UserID] = @UserID";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        Command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = UserName;
                        Command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = Password;
                        Command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = IsActive;
                        Command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

                        connection.Open();
                        NumberOfEffectedRows = Command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to update user ID: {UserID}");
            }

            return NumberOfEffectedRows > 0;
        }

        public static bool IsUserExists(int UserID)
        {
            bool IsExist = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"SELECT TOP (1) CAST(1 AS BIT)
                                     FROM Users
                                     WHERE UserID = @UserID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

                        connection.Open();
                        if (command.ExecuteScalar() is bool found)
                        {
                            IsExist = found;
                        }
                        else
                        {
                            IsExist = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check user existence by UserID: {UserID}");
                IsExist = false;
            }

            return IsExist;
        }

        public static bool IsUserExists(string UserName)
        {
            bool IsExist = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"SELECT TOP (1) CAST(1 AS BIT)
                                     FROM Users
                                     WHERE UserName = @UserName";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = UserName;

                        connection.Open();
                        if (command.ExecuteScalar() is bool found)
                        {
                            IsExist = found;
                        }
                        else
                        {
                            IsExist = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check user existence by UserName: {UserName}");
                IsExist = false;
            }

            return IsExist;
        }

        public static bool IsUserExistForPersonID(int PersonID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT TOP (1) CAST(1 AS BIT)
                                     FROM Users
                                     WHERE PersonID = @PersonID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        connection.Open();
                        if (command.ExecuteScalar() is bool found)
                        {
                            isFound = found;
                        }
                        else
                        {
                            isFound = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check user existence for PersonID: {PersonID}");
                isFound = false;
            }

            return isFound;
        }

        public static bool DeleteUser(int UserID)
        {
            int NumberOfEffectedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"DELETE FROM Users
                                     WHERE UserID = @UserID";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        Command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

                        connection.Open();
                        NumberOfEffectedRows = Command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to delete user ID: {UserID}");
            }

            return NumberOfEffectedRows > 0;
        }

        public static DataTable GetAllUsers()
        {
            DataTable Table = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"SELECT Users.UserID,
                                            Users.PersonID,
                                            People.FirstName + ' ' + People.SecondName + ' ' + ISNULL(People.ThirdName, '') + ' ' + People.LastName AS FullName,
                                            Users.UserName,
                                            Users.IsActive
                                     FROM Users
                                     INNER JOIN People ON Users.PersonID = People.PersonID";

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
                clsLogger.LogException(ex, "Failed to retrieve all users.");
            }

            return Table;
        }

        public static bool DoesPersonHaveUser44(int PersonID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT TOP (1) CAST(1 AS BIT)
                                     FROM Users
                                     WHERE PersonID = @PersonID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        connection.Open();
                        if (command.ExecuteScalar() is bool found)
                        {
                            isFound = found;
                        }
                        else
                        {
                            isFound = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to check DoesPersonHaveUser44 for PersonID: {PersonID}");
                isFound = false;
            }

            return isFound;
        }

        public static bool ChangeUserCredentials(int UserID, string NewUserName, string NewPassword)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"UPDATE Users
                                     SET UserName = @UserName,
                                         [Password] = @Password
                                     WHERE UserID = @UserID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                        command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = NewUserName;
                        command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = NewPassword;

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to change user credentials for UserID: {UserID}");
                return false;
            }

            return (rowsAffected > 0);
        }

        public static bool ChangePassword(int UserID, string NewPassword)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"UPDATE Users
                                     SET [Password] = @Password
                                     WHERE UserID = @UserID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                        command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = NewPassword;

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to change password for UserID: {UserID}");
                return false;
            }

            return (rowsAffected > 0);
        }

        public static bool ChangeUserActivity(int UserID, bool IsActive)
        {
            bool IsSucceed = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"UPDATE [Users]
                                     SET [IsActive] = @IsActive
                                     WHERE UserID = @UserID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = IsActive;

                        connection.Open();
                        IsSucceed = (command.ExecuteNonQuery() > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to change user activity for UserID: {UserID}");
                IsSucceed = false;
            }

            return IsSucceed;
        }

        public static bool HasUsers()
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = @"SELECT TOP (1) CAST(1 AS BIT)
                                     FROM Users;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        if (command.ExecuteScalar() is bool found)
                        {
                            isFound = found;
                        }
                        else
                        {
                            isFound = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Failed to check users existence.");
                isFound = false;
            }

            return isFound;
        }

        public static int GetUserPermissionsByUserID(int UserID)
        {
            int Permissions = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string Query = @"SELECT
                                    Permission
                                 FROM Users WHERE UserID = @UserID;";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SequentialAccess))
                        {
                            if (reader.Read())
                            {
                                Permissions = reader.GetInt32(0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to retrieve User Permissions for UserID: {UserID}");
            }

            return Permissions;
        }


    }
}
