using DVLD_Shared;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DVLD_Infrastructure.Storage;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsUserData
    {
        public static async Task<(bool IsFound, int PersonID, string UserName, string Password, bool IsActive)> GetUserInfoByUserIDAsync(int UserID)
        {
            bool IsFound = false;
            int PersonID = -1;
            string UserName = string.Empty;
            string Password = string.Empty;
            bool IsActive = false;
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
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

            return (IsFound, PersonID, UserName, Password, IsActive);
        }

        public static async Task<(bool IsFound, int UserID, int PersonID, string Password, bool IsActive)> GetUserInfoByUserNameAsync(string UserName)
        {
            bool IsFound = false;
            int UserID = -1;
            int PersonID = -1;
            string Password = string.Empty;
            bool IsActive = false;
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
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

            return (IsFound, UserID, PersonID, Password, IsActive);
        }

        public static async Task<(bool IsFound, int UserID, string UserName, string Password, bool IsActive)> GetUserInfoByPersonIDAsync(int PersonID)
        {
            bool IsFound = false;
            int UserID = -1;
            string UserName = string.Empty;
            string Password = string.Empty;
            bool IsActive = false;
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
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

            return (IsFound, UserID, UserName, Password, IsActive);
        }

        public static async Task<(bool IsFound, int UserID, int PersonID, bool IsActive)> GetUserInfoByUsernameAndPasswordAsync(string UserName, string Password)
        {
            bool isFound = false;
            int UserID = -1;
            int PersonID = -1;
            bool IsActive = false;
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
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

            return (isFound, UserID, PersonID, IsActive);
        }


        public static async Task<int> AddNewUserAsync(int PersonID, string UserName,
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is int NewID)
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

        public static async Task<bool> UpdateUserAsync(int UserID, string UserName,
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        NumberOfEffectedRows = await Command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to update user ID: {UserID}");
            }

            return NumberOfEffectedRows > 0;
        }

        public static async Task<bool> IsUserExistsAsync(int UserID)
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is bool found)
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

        public static async Task<bool> IsUserExistsAsync(string UserName)
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is bool found)
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

        public static async Task<bool> IsUserExistForPersonIDAsync(int PersonID)
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is bool found)
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

        public static async Task<bool> DeleteUserAsync(int UserID)
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        NumberOfEffectedRows = await Command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Failed to delete user ID: {UserID}");
            }

            return NumberOfEffectedRows > 0;
        }

        public static async Task<DataTable> GetAllUsersAsync()
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
                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await Command.ExecuteReaderAsync().ConfigureAwait(false))
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

        public static async Task<bool> DoesPersonHaveUser44Async(int PersonID)
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is bool found)
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

        public static async Task<bool> ChangeUserCredentialsAsync(int UserID, string NewUserName, string NewPassword)
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
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

        public static async Task<bool> ChangePasswordAsync(int UserID, string NewPassword)
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
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

        public static async Task<bool> ChangeUserActivityAsync(int UserID, bool IsActive)
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        IsSucceed = (await command.ExecuteNonQueryAsync().ConfigureAwait(false) > 0);
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

        public static async Task<bool> HasUsersAsync()
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
                        await connection.OpenAsync().ConfigureAwait(false);
                        if (await command.ExecuteScalarAsync().ConfigureAwait(false) is bool found)
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

        public static async Task<int> GetUserPermissionsByUserIDAsync(int UserID)
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

                        await connection.OpenAsync().ConfigureAwait(false);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow | CommandBehavior.SequentialAccess).ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
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
