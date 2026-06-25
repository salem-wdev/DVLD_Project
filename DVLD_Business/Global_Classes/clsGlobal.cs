using System;
using System.IO;

namespace DVLD_Business.Global_Classes
{
    public static class clsGlobal
    {
        public static clsUser CurrentUser;

        private static readonly string _filePath 
            = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                , "DVLD_Project\\data.txt");

        public static bool RememberUsernameAndPassword(string Username, string Password)
        {
            try
            {
                string folderPath = Path.GetDirectoryName(_filePath);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                if (Username == "" && File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                    return false;
                }

                string DataToSave = Username + "#//#" + Password;

                using (StreamWriter writer = new StreamWriter(_filePath))
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



                if (File.Exists(_filePath))
                {
                    using (StreamReader reader = new StreamReader(_filePath))
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
