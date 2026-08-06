using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Shared.Storage
{
    public static class clsRegistryManager
    {
        public static bool RegisterValues(Dictionary<string,string> values, string subKeyPath)
        {
            string failedKey = string.Empty;
            try
            {
                using (RegistryKey Key = Registry.CurrentUser.CreateSubKey(subKeyPath))
                {
                    if (Key != null)
                    {
                        foreach (var kvp in values)
                        {
                            failedKey = kvp.Key;
                            Key.SetValue(kvp.Key, kvp.Value, RegistryValueKind.String);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error registering Key in registry for {failedKey}");
                DeleteValues(values.Keys.ToArray(), subKeyPath);
                return false;
            }
            return true;
        }

        private static bool DeleteValues(string[] keys, string subKeyPath)
        {
            string failedKey = string.Empty;
            try
            {
                using (RegistryKey Key = Registry.CurrentUser.OpenSubKey(subKeyPath, true))
                {
                    if (Key != null)
                    {
                        foreach (var kvp in keys)
                        {
                            failedKey = kvp;
                            Key.DeleteValue(kvp, false);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error deleting Key from registry for {failedKey}");
                return false;
            }
            return true;
        }

        public static Dictionary<string, string> GetRegisteredValues(string[] keys, string subKeyPath)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string failedKey = string.Empty;
            try
            {
                using (RegistryKey Key = Registry.CurrentUser.OpenSubKey(subKeyPath))
                {
                    if (Key != null)
                    {
                        foreach (var kvp in keys)
                        {
                            failedKey = kvp;
                            var value = Key.GetValue(kvp) as string;
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                result[kvp] = value as string;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error retrieving key from registry for {failedKey}");
            }
            return result;
        }

    }
}
