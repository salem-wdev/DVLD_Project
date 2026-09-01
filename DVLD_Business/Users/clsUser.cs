using DVLD_Infrastructure.Storage;
using DVLD_DataAccess;
using DVLD_Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_Shared.Users;
using DVLD_Shared.Utilities;

namespace DVLD_Business.Users
{
    public class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; private set; }
        private readonly Dictionary<enMode, Func<Task<bool>>> _saveDictionary;

        public int UserID { get; private set; }
        public int PersonID { get; private set; }

        private clsPerson _Person;

        public clsPerson PersonInfo
        {
            get
            {
                if (_Person == null && PersonID != -1)
                {
                    _FindPerson();
                }
                return _Person;
            }
        }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public bool IsActive { get; set; }


        private clsUser(int PersonID, string UserName, string Password)
        {
            this.UserID = -1;
            this.PersonID = PersonID;
            this.UserName = UserName;
            this.Password = Password;
            this.IsActive = true;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
              {enMode.AddNew, _AddNewUserAsync },
              {enMode.Update, _UpdateUserAsync},
            };

            Mode = enMode.AddNew;
        }


        // New overload that sets UserID so instances returned from Find have correct ID
        private clsUser(int UserID, int PersonID, string UserName,
      string Password, bool IsActive)
        {
            this.UserID = UserID;
            this.PersonID = PersonID;
            this.UserName = UserName;
            this.Password = Password;
            this.IsActive = IsActive;

            _saveDictionary = new Dictionary<enMode, Func<Task<bool>>>
            {
              {enMode.AddNew, _AddNewUserAsync },
              {enMode.Update, _UpdateUserAsync},
            };

            Mode = enMode.Update;
        }

        private async Task<bool> _AddNewUserAsync()
        {
            //if (!_Person.Save()) // Ensure the person is saved and has a valid PersonID
            //{
            //    return false;
            //}

            this.UserID = await clsUserData.AddNewUserAsync(this.PersonID, this.UserName,
        this.Password, this.IsActive);

            if (UserID != -1)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private async Task<bool> _UpdateUserAsync()
        {
            //if (!_Person.Save()) // Ensure the person is saved and has a valid PersonID
            //{
            //    return false;
            //}

            return await clsUserData.UpdateUserAsync(this.UserID, this.UserName,
        this.Password, this.IsActive);
        }

        private async void _FindPerson()
        {
            _Person = await clsPerson.FindAsync(PersonID).ConfigureAwait(false);
        }

        public static async Task<bool> DeleteAsync(int UserID)
        {
            return await clsUserData.DeleteUserAsync(UserID);
        }

        public static async Task<clsUser> FindAsync(int UserID)
        {
            var result = await clsUserData.GetUserInfoByUserIDAsync(UserID);

            if (result.IsFound)
            {
                return new clsUser(UserID, result.PersonID, result.UserName,
                  result.Password, result.IsActive);
            }
            else
            {
                return null;
            }
        }

        public static async Task<clsUser> FindByPersonIDAsync(int PersonID)
        {
           
           var result = await clsUserData.GetUserInfoByPersonIDAsync(PersonID);

            if (result.IsFound)
            {
                return new clsUser(result.UserID, PersonID, result.UserName,
                  result.Password, result.IsActive);
            }
            else
            {
                return null;
            }
        }

        public static async Task<clsUser> FindAsync(string UserName)
        {
            var result = await clsUserData.GetUserInfoByUserNameAsync(UserName);

            if (result.IsFound)
            {
                return new clsUser(result.UserID, result.PersonID, UserName,
                  result.Password, result.IsActive);
            }
            else
            {
                return null;
            }
        }

        protected static async Task<clsUser> FindByUsernameAndPasswordAsync(string UserName, string Password)
        {
            string HashedPassord = string.Empty;

            try
            {
                HashedPassord = clsCryptoHelper.ComputeHash(Password);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex,$"Error computing hash for password of Username {UserName}");
                return null;
            }

            var result = await clsUserData.GetUserInfoByUsernameAndPasswordAsync(UserName, HashedPassord);
            if (result.IsFound)
            {
                return new clsUser(result.UserID, result.PersonID,
                    UserName, Password, result.IsActive);
            }
            else
            {
                return null;
            }
        }

        public static async Task<bool> IsUserExistsAsync(int UserID)
        {
            return await clsUserData.IsUserExistsAsync(UserID);
        }

        public static async Task<bool> IsUserExistsAsync(string UserName)
        {
            return await clsUserData.IsUserExistsAsync(UserName);
        }

        public static async Task<bool> IsUserExistsForPersonIDAsync(int PersonID)
        {
            return await clsUserData.IsUserExistForPersonIDAsync(PersonID);
        }

        public static async Task<DataTable> GetAllUsersAsync()
        {
            return await clsUserData.GetAllUsersAsync();
        }
        // TODO: Consider moving validation logic for IsActive/Credentials to a centralized 
        // 'ValidateBusinessRules()' method within clsUser before calling Save().
        public async Task<bool> SaveAsync()
        {
            return await _saveDictionary[this.Mode]();
        }

        public async Task<bool> ChangeUserCredentialsAsync(string NewUserName, string NewPassword)
        {
            var result = await ChangeUserCredentialsAsync(this.UserID, NewUserName, NewPassword);
            if (result.IsSucceed)
            {
                this.UserName = NewUserName;
                this.Password = result.HashedPassword;
                return true;
            }

            return false;
        }

        public async Task<bool> ChangeUserActivityAsync(bool IsActive)
        {
            if (await ChangeUserActivityAsync(this.UserID, IsActive))
            {
                this.IsActive = IsActive;
                return true;
            }

            return false;
        }

        public static async Task<(bool IsSucceed, string HashedPassword)>
            ChangeUserCredentialsAsync(int UserID, string NewUserName, string NewPassword)
        {
            try
            {
                NewPassword = clsCryptoHelper.ComputeHash(NewPassword);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error computing hash for new password of user ID {UserID} Username {NewUserName}");
                return (false, string.Empty);
            }

            bool isSucceed = await clsUserData.ChangeUserCredentialsAsync(UserID, NewUserName, NewPassword);

            return (isSucceed, NewPassword);
        }

        public static async Task<bool> ChangePasswordAsync(int UserID, string NewPassword)
        {
            string HashedPassord = string.Empty;
            try
            {
                HashedPassord = clsCryptoHelper.ComputeHash(NewPassword);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error computing hash for new password of user ID {UserID}");
                return false;
            }

            return await clsUserData.ChangePasswordAsync(UserID, HashedPassord);
        }

        public static async Task<bool> DoesPersonHaveUserAsync(int PersonID)
        {
            return await clsUserData.DoesPersonHaveUser44Async(PersonID);
        }

        public static async Task<bool> ChangeUserActivityAsync(int UserID, bool IsActive)
        {
            return await clsUserData.ChangeUserActivityAsync(UserID, IsActive);
        }

        public static async Task<clsUser> CreateNewUserAsync(int PersonID, string UserName, string Password)
        {
            if (!await clsPerson.IsPersonExistsAsync(PersonID) || await clsUser.IsUserExistsForPersonIDAsync(PersonID))
            {
                return null;
            }

            if (await clsUser.IsUserExistsAsync(UserName))
            {
                return null;
            }

            if (await IsUserExistsForPersonIDAsync(PersonID))
            {
                return null;
            }

            string HashedPassord = string.Empty;
            try
            {

                HashedPassord = clsCryptoHelper.ComputeHash(Password);

            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex,$"Error computing hash for password of person ID {PersonID} Username {UserName}");
                return null;
            }
            return new clsUser(PersonID, UserName, HashedPassord);
        }

        public static async Task<clsUser> LoginAsync(string UserName, string Password)
        {

            clsUser user = await clsUser.FindByUsernameAndPasswordAsync(UserName, Password);

            //incase the user is null or not active.
            if (user == null || !user.IsActive)
            {
                return null;
            }
            return user;
        }

        public static async Task<bool> HasUsersAsync()
        {
            return await clsUserData.HasUsersAsync();
        }

        public static async Task<enUserPermissions> GetUserPermissionsAsync(int UserID)
        {
            return (enUserPermissions)await clsUserData.GetUserPermissionsByUserIDAsync(UserID);
        }

        public static async Task<bool> ValidationUserAsync(int UserID)
        {
            enUserPermissions userPermissionsValue = await GetUserPermissionsAsync(UserID);
            return clsPermissionEvaluator.ValidationUser(userPermissionsValue);
        }

        public async Task<bool> ValidationUserAsync()
        {
            enUserPermissions userPermissionsValue = await GetUserPermissionsAsync(this.UserID);
            return clsPermissionEvaluator.ValidationUser(userPermissionsValue);
        }
    }
}