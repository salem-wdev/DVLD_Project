using DVLD_Business.Global_Classes;
using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; private set; }

        public int UserID { get; private set; }
        public int PersonID { get; private set; }

        private clsPerson _Person; 

        public clsPerson PersonInfo
        {
            get
            {
                if (_Person == null && PersonID != -1)
                {
                    _Person = clsPerson.Find(this.PersonID);
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

            return (UserID != -1);
        }

        private bool _UpdateUser()
        {
            //if (!_Person.Save()) // Ensure the person is saved and has a valid PersonID
            //{
            //    return false;
            //}

            return clsUserData.UpdateUser(this.UserID, this.UserName,
                this.Password, this.IsActive);
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

            bool found = clsUserData.GetUserInfoByUsernameAndPassword(UserName,
                Password, ref UserID, ref PersonID, ref IsActive);
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
            switch (Mode)
            {
                case enMode.AddNew:
                    {
                        if (_AddNewUser())
                        {
                            Mode = enMode.Update;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case enMode.Update:
                    {
                        return _UpdateUser();
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public bool ChangeUserCredentials(string NewUserName, string NewPassword)
        {
            if (ChangeUserCredentials(this.UserID, NewUserName, NewPassword))
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

        public static bool ChangeUserCredentials(int UserID, string NewUserName, string NewPassword)
        {
            return clsUserData.ChangeUserCredentials(UserID, NewUserName, NewPassword);
        }

        public static bool ChangePassword(int UserID, string NewPassword)
        {
            return clsUserData.ChangePassword(UserID, NewPassword);
        }

        public static bool DoesPersonHaveUser(int PersonID)
        {
            return clsUserData.DoesPersonHaveUser44(PersonID);
        }

        public static bool ChangeUserActivity(int UserID, bool IsActive)
        {
            return clsUserData.ChangeUserActivity(UserID, IsActive);
        }

        public static clsUser CreateNewUser(int PersonID, string UserName, string Password)
        {
            if (!clsPerson.IsPersonExists(PersonID) || clsUser.IsUserExistsForPersonID(PersonID))
            {
                return null;
            }

            if(clsUser.IsUserExists(UserName))
            {
                return null;
            }

            return new clsUser(PersonID, UserName, Password);
        }

        public static clsUser Login(string UserName, string Password, bool IsRememberd)
        {
            clsUser user = clsUser.FindByUsernameAndPassword(UserName, Password);

            if (user != null)
            {
                if (IsRememberd)
                {
                    //store username and password
                    clsGlobal.RememberUsernameAndPassword(UserName.Trim(), Password.Trim());

                }
                else
                {
                    //store empty username and password
                    clsGlobal.RememberUsernameAndPassword("", "");

                }

                //incase the user is not active
                if (!user.IsActive)
                {
                    return null;
                }
                return user;
            }
            else
            {
                return null;
            }
        }

        public static bool HasUsers()
        {
            return clsUserData.HasUsers();
        }

    }
}
