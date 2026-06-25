using DVLD_Business;
using System;
using System.IO;

namespace DVLD.Global_Classes
{
    public static class clsGlobal
    {
        public static clsUser CurrentUser;

        public static bool RememberUsernameAndPassword(string Username, string Password)
        {
            try
            {
                string currentDirectory = System.IO.Directory.GetCurrentDirectory();

                string filePath = currentDirectory + "\\data.txt";

                if (Username == "" && File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return false;
                }

                string DataToSave = Username + "#//#" + Password;

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(DataToSave);
                    return true;
                }

            }
            catch (Exception ex)
            {
            }

            return false;
        }

        public static bool GetStoredCredential(ref string Username, ref string Password)
        {
            try
            {

                string currentDirectory = System.IO.Directory.GetCurrentDirectory();

                string filePath = currentDirectory + "\\data.txt";

                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            Console.WriteLine(line);
                            string[] strings = line.Split(new string[] { "#//#" }, StringSplitOptions.None);
                            Username = strings[0];
                            Password = strings[1];
                        }
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return false;

        }

        public static DateTime GetServerDateTime()
        {
            return clsBusinessSettings.GetServerDateTime();
        }
    }
}
