using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_Shared;

namespace DVLD_Infrastructure.Storage
{
    public class clsFileStorage
    {
        public static bool CreateFolderIfDoesNotExist(string FolderPath)
        {

            // Check if the folder exists
            if (!Directory.Exists(FolderPath))
            {
                try
                {
                    // If it doesn't exist, create the folder
                    Directory.CreateDirectory(FolderPath);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;

        }

        public static string ReplaceFileNameWithGUID(string sourceFile)
        {
            // Full file name. Change your file name   
            string fileName = sourceFile;
            FileInfo fi = new FileInfo(fileName);
            string extn = fi.Extension;
            return clsUtil.GenerateGUID() + extn;

        }

        public static bool CopyFileToDestinationFolderWithGUID(ref string sourceFile, string DestinationFolder)
        {
            // this funciton will copy the image to the
            // project images foldr after renaming it
            // with GUID with the same extention, then it will update the sourceFileName with the new name.

            if (!CreateFolderIfDoesNotExist(DestinationFolder))
            {
                return false;
            }

            string destinationFile = DestinationFolder + ReplaceFileNameWithGUID(sourceFile);
            try
            {
                File.Copy(sourceFile, destinationFile, true);

            }
            catch (IOException iox)
            {
                return false;
            }

            sourceFile = destinationFile;
            return true;
        }
       
        public static bool DeleteFile(string sourceFile)
        {
            try
            {
                File.Delete(sourceFile);
                return true;
            }
            catch (Exception iox)
            {
                return false;
            }
        }
    }
}
