using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_Shared.Storage;

namespace DVLD_Business.Users
{
    public static class clsUserSettings
    {
        private const string _RegistryPath = @"SOFTWARE\DVLD";
        private const string _UsernameKey = "Username";
        private const string _PasswordKey = "Password";

        public static bool RememberMe(string userName, string password)
        {
            // Store the "RememberMe" value in the registry
            return clsRegistryManager.RegisterValues(
                 new Dictionary<string, string>
                 {
                    { _UsernameKey, userName.Trim() },
                    { _PasswordKey, password.Trim() }
                 }
                 , _RegistryPath);
        }

        public static bool RemoveRememberedCredentials()
        {
            return clsRegistryManager.DeleteValues(new[] { _UsernameKey, _PasswordKey }, _RegistryPath);
        }

        public static (string Username, string Password) GetRememberedCredentials()
        {
            var values = clsRegistryManager.GetRegisteredValues(new[] { _UsernameKey, _PasswordKey }, _RegistryPath);
            if (values != null && values.Count == 2)
            {
                return (values[_UsernameKey], values[_PasswordKey]);
            }
            return (string.Empty, string.Empty);
        }
    }
}
