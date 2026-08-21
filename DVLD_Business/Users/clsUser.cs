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
        private readonly Dictionary<enMode, Func<bool>> _saveDictionary;

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

            _saveDictionary = new Dictionary<enMode, Func<bool>>
      {
        {enMode.AddNew, _AddNewUser },
        {enMode.Update, _UpdateUser},
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

            _saveDictionary = new Dictionary<enMode, Func<bool>>
      {
        {enMode.AddNew, _AddNewUser },
        {enMode.Update, _UpdateUser},
      };

            Mode = enMode.Update;
        }

        private bool _AddNewUser()
        {
            //if (!_Person.Save()) // Ensure the person is saved and has a valid PersonID
            //{
            //    return false;
            //}

            this.UserID = clsUserData.AddNewUser(this.PersonID, this.UserName,
        this.Password, this.IsActive);

            if (UserID != -1)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private bool _UpdateUser()
        {
            //if (!_Person.Save()) // Ensure the person is saved and has a valid PersonID
            //{
            //    return false;
            //}

            return clsUserData.UpdateUser(this.UserID, this.UserName,
        this.Password, this.IsActive);
        }

        private async void _FindPerson()
        {
            _Person = await clsPerson.FindAsync(PersonID).ConfigureAwait(false);
        }

        public static bool Delete(int UserID)
        {
            return clsUserData.DeleteUser(UserID);
        }

        public static clsUser Find(int UserID)
        {
            int PersonID = -1;
            string UserName = string.Empty;
            string Password = string.Empty;
            bool IsActive = false;


            bool found = clsUserData.GetUserInfoByUserID(UserID, ref PersonID,
              ref UserName, ref Password, ref IsActive);

            if (found)
            {
                return new clsUser(UserID, PersonID, UserName,
                  Password, IsActive);

            }
            else
            {
                return null;
            }
        }

        public static clsUser FindByPersonID(int PersonID)
        {
            int UserID = -1;
            string UserName = string.Empty;
            string Password = string.Empty;
            bool IsActive = false;

            bool found = clsUserData.GetUserInfoByPersonID(PersonID, ref UserID,
              ref UserName, ref Password, ref IsActive);

            if (found)
            {
                return new clsUser(UserID, PersonID, UserName, Password, IsActive);
            }
            else
            {
                return null;
            }
        }

        public static clsUser Find(string UserName)
        {
            int UserID = -1;
            int PersonID = -1;
            string Password = string.Empty;
            bool IsActive = false;


            bool found = clsUserData.GetUserInfoByUserName(UserName,
              ref UserID, ref PersonID, ref Password, ref IsActive);

            if (found)
            {
                return new clsUser(UserID, PersonID, UserName, Password, IsActive);
            }
            else
            {
                return null;
            }
        }

        protected static clsUser FindByUsernameAndPassword(string UserName, string Password)
        {
            int UserID = -1;
            int PersonID = -1;
            bool IsActive = false;
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

            bool found = clsUserData.GetUserInfoByUsernameAndPassword(UserName,
              HashedPassord, ref UserID, ref PersonID, ref IsActive);
            if (found)
            {
                return new clsUser(UserID, PersonID, UserName, Password, IsActive);
            }
            else
            {
                return null;
            }
        }

        public static bool IsUserExists(int UserID)
        {
            return clsUserData.IsUserExists(UserID);
        }

        public static bool IsUserExists(string UserName)
        {
            return clsUserData.IsUserExists(UserName);
        }

        public static bool IsUserExistsForPersonID(int PersonID)
        {
            return clsUserData.IsUserExistForPersonID(PersonID);
        }

        public static DataTable GetAllUsers()
        {
            return clsUserData.GetAllUsers();
        }
        // TODO: Consider moving validation logic for IsActive/Credentials to a centralized 
        // 'ValidateBusinessRules()' method within clsUser before calling Save().
        public bool Save()
        {
            return _saveDictionary[this.Mode]();
        }

        public bool ChangeUserCredentials(string NewUserName, string NewPassword)
        {
            if (ChangeUserCredentials(this.UserID, NewUserName, ref NewPassword))
            {
                this.UserName = NewUserName;
                this.Password = NewPassword;
                return true;
            }

            return false;
        }

        public bool ChangeUserActivity(bool IsActive)
        {
            if (ChangeUserActivity(this.UserID, IsActive))
            {
                this.IsActive = IsActive;
                return true;
            }

            return false;
        }

        public static bool ChangeUserCredentials(int UserID, string NewUserName, ref string NewPassword)
        {
            try
            {
                NewPassword = clsCryptoHelper.ComputeHash(NewPassword);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error computing hash for new password of user ID {UserID} Username {NewUserName}");
                return false;
            }

            return clsUserData.ChangeUserCredentials(UserID, NewUserName, NewPassword);
        }

        public static bool ChangePassword(int UserID, string NewPassword)
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

            return clsUserData.ChangePassword(UserID, HashedPassord);
        }

        public static bool DoesPersonHaveUser(int PersonID)
        {
            return clsUserData.DoesPersonHaveUser44(PersonID);
        }

        public static bool ChangeUserActivity(int UserID, bool IsActive)
        {
            return clsUserData.ChangeUserActivity(UserID, IsActive);
        }

        public static async Task<clsUser> CreateNewUserAsync(int PersonID, string UserName, string Password)
        {
            if (!await clsPerson.IsPersonExistsAsync(PersonID) || clsUser.IsUserExistsForPersonID(PersonID))
            {
                return null;
            }

            if (clsUser.IsUserExists(UserName))
            {
                return null;
            }

            if (IsUserExistsForPersonID(PersonID))
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

        public static clsUser Login(string UserName, string Password)
        {

            clsUser user = clsUser.FindByUsernameAndPassword(UserName, Password);

            //incase the user is null or not active.
            if (user == null || !user.IsActive)
            {
                return null;
            }
            return user;
        }

        public static bool HasUsers()
        {
            return clsUserData.HasUsers();
        }

        public static enUserPermissions GetUserPermissions(int UserID)
        {
            return (enUserPermissions)clsUserData.GetUserPermissionsByUserID(UserID);
        }

        public static bool ValidationUser(int UserID)
        {
            enUserPermissions userPermissionsValue = GetUserPermissions(UserID);
            return clsPermissionEvaluator.ValidationUser(userPermissionsValue);
        }

        public bool ValidationUser()
        {
            enUserPermissions userPermissionsValue = GetUserPermissions(this.UserID);
            return clsPermissionEvaluator.ValidationUser(userPermissionsValue);
        }
    }
}