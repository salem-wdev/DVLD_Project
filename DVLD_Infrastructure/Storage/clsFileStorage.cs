using DVLD_Shared;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DVLD_Infrastructure.Storage
{
    public static class clsFileStorage
    {
        public static bool CreateFolderIfDoesNotExist(string FolderPath)
        {
            if(string.IsNullOrWhiteSpace(FolderPath)) 
                return false;

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
                    clsLogger.LogException(ex, $"I/O error Creating Folder in path {FolderPath}");
                    return false;
                }
            }

            return true;

        }

        public static string ReplaceFileNameWithGUID(string sourceFile)
        {
            if (string.IsNullOrWhiteSpace(sourceFile))
                return null;
            // Full file name. Change your file name   

            string extn = Path.GetExtension(sourceFile);
            return clsUtil.GenerateGUID() + extn;

        }

        public static async Task<string> CopyFileToDestinationFolderWithGUIDAsync(string sourceFile, string destinationFolder)
        {
            if (string.IsNullOrWhiteSpace(sourceFile) || string.IsNullOrWhiteSpace(destinationFolder))
                return null;

            // this funciton will copy the image to the
            // project images foldr after renaming it
            // with GUID with the same extention, then it will update the sourceFileName with the new name.

            if (!File.Exists(sourceFile))
                return null;


            if (!CreateFolderIfDoesNotExist(destinationFolder))
            {
                return null;
            }

            string destinationFileNameWithGUID = ReplaceFileNameWithGUID(sourceFile);

            if(destinationFileNameWithGUID == null)
                return null;

            string destinationFile = Path.Combine(destinationFolder, destinationFileNameWithGUID);
            try
            {
                using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                using (var destinationStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"I/O error while copying file from '{sourceFile}' to '{destinationFile}' in {nameof(CopyFileToDestinationFolderWithGUIDAsync)}.");
                DeleteFile(destinationFile);
                return null;
            }

            return destinationFile;
        }

        public static bool DeleteFile(string sourceFile)
        {
            if(string.IsNullOrWhiteSpace(sourceFile))
                return false;

            if (!File.Exists(sourceFile))
                return false;

            try
            {
                File.Delete(sourceFile);
                return true;
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"I/O error while deleting file '{sourceFile}'.");
                return false;
            }
        }
    }
}
