using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DVLD_Infrastructure.Storage
{
    public static class clsLogger
    {
        private static readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_errors.log");

        public enum LogLevel { Info, Warning, Error }

        [Conditional("DEBUG")]
        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            string formattedMessage = $"[{level.ToString().ToUpper()}] {DateTime.Now}: {message}";
            System.Diagnostics.Debug.WriteLine(formattedMessage);
        }

        /// <summary>
        /// Logs critical exceptions into a persistent file.
        /// This method executes in both Debug and Release configurations.
        /// </summary>
        /// <param name="ex">The exception object containing details and stack trace.</param>
        /// <param name="customMessage">An optional context message describing where or why the error occurred.</param>
        public static void LogException(Exception ex, string customMessage = "")
        {
            try
            {
                // Format a detailed and professional log entry for debugging production issues
                string logEntry = $"==================================================\n" +
                                  $"[CRITICAL ERROR] {DateTime.Now}\n" +
                                  $"Context: {customMessage}\n" +
                                  $"Message: {ex.Message}\n" +
                                  $"Source: {ex.Source}\n" +
                                  $"Stack Trace:\n{ex.StackTrace}\n" +
                                  $"==================================================\n\n";

                // Append the entry to the log file (Creates the file if it doesn't exist)
                File.AppendAllText(_logFilePath, logEntry);
            }
            catch
            {
                // Empty catch block ensures that if logging fails (e.g., due to file permissions),
                // it won't crash the main application.
            }
        }
    }
}
