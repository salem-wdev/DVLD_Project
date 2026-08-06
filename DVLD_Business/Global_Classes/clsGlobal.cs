using System;
using System.Collections.Generic;
using DVLD_Business;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_Business.Users;
using Microsoft.Win32;
using DVLD_Shared;


namespace DVLD_Business.Global_Classes
{
    public static class clsGlobal
    {
        public static clsUser CurrentUser;

        private static readonly string _SubKeyPath = @"SOFTWARE\DVLD";

        private static bool RegisterCredentials(string username, string password)
        {
            try
            {
                using (RegistryKey Key = Registry.CurrentUser.CreateSubKey(_SubKeyPath))
                {
                    if (Key != null)
                    {
                        Key.SetValue("Username", username, RegistryValueKind.String);
                        Key.SetValue("Password", password, RegistryValueKind.String);
                    }
                    else
                    { 
                    return false;
                    }
                }
            }
            catch(Exception ex)
            {
                clsLogger.LogException(ex, $"Error registering credentials in registry for {username}");
                DeleteCredentials();
                return false;
            }
            return true;
        }

        private static bool DeleteCredentials()
        {
            try
            {
                using (RegistryKey Key = Registry.CurrentUser.OpenSubKey(_SubKeyPath, true))
                {
                    if (Key != null)
                    {
                        Key.DeleteValue("Username", false);
                        Key.DeleteValue("Password", false);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error deleting credentials from registry");
                return false;
            }
            return true;
        }

        internal static bool RememberUsernameAndPassword(string Username, string Password, bool Remember)
        {
            if (Remember)
            {
                return RegisterCredentials(Username, Password);
            }
            else
            {
                return DeleteCredentials();
            }
        }

        public static bool GetStoredCredential(ref string username, ref string password)
        {
            try
            {
                using (RegistryKey Key = Registry.CurrentUser.OpenSubKey(_SubKeyPath))
                {
                    if (Key != null)
                    {
                        username = Key.GetValue("Username") as string;
                        password = Key.GetValue("Password") as string;

                        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                        {
                            username = null;
                            password = null;
                            return false;
                        }
                    }
                    else
                    {
                        username = null;
                        password = null;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error retrieving credentials from registry");
                username = null;
                password = null;
                return false;
            }
            return true;
        }
        
    }
}
