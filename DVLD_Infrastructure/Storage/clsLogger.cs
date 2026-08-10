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
        private static readonly object _fileLock = new object();

        public enum LogLevel { Info, Warning, Error }

        [Conditional("DEBUG")]
        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
                string formattedMessage = $"[{level.ToString().ToUpper()}] {DateTime.Now}: {message}";
                System.Diagnostics.Debug.WriteLine(formattedMessage);
        }

        private static void LogLoggerError(string message)
        {
            try
            {
                string logEntry = $"[LOGGER ERROR] {DateTime.Now}: {message}\n";
                lock (_fileLock)
                {
                    File.AppendAllText(_logFilePath, logEntry);
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"[CRITICAL LOGGER FINAL FAILURE]: {message}");
            }
        }

        private static bool CreatEventLogSource(string sourceName, string logName = "Application")
        {
            try
            {
                if (!EventLog.SourceExists(sourceName))
                {
                    EventLog.CreateEventSource(sourceName, logName);
                }
                return EventLog.SourceExists(sourceName);
            }
            catch (Exception ex)
            {
                LogLoggerError($"Failed to create event log source '{sourceName}' in log '{logName}'. Exception: {ex.Message}");
                return false;
            }
        }

        public static void LogToEventLog(string sourceName, string message, EventLogEntryType entryType = EventLogEntryType.Information)
        {
            try
            {
                if (CreatEventLogSource(sourceName))
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = sourceName;
                        eventLog.WriteEntry(message, entryType);
                    }
                }
            }
            catch (Exception ex)
            {
                LogLoggerError($"Failed to log message to event log with source '{sourceName}' for {message}. Exception: {ex.Message}");
            }
        }

        public static void LogToFile(Exception OutEx, string customMessage = "")
        {
            // Format a detailed and professional log entry for debugging production issues
            string logEntry = $"==================================================\n" +
                              $"[CRITICAL ERROR] {DateTime.Now}\n" +
                              $"Context: {customMessage}\n" +
                              $"Message: {OutEx.Message}\n" +
                              $"Source: {OutEx.Source}\n" +
                              $"Stack Trace:\n{OutEx.StackTrace}\n" +
                              $"==================================================\n\n";

            try
            {
                lock (_fileLock)
                {  // Append the entry to the log file (Creates the file if it doesn't exist)
                    File.AppendAllText(_logFilePath, logEntry);
                }
            }
            catch (Exception ex)
            {
                LogLoggerError($"Failed to write log entry to file for {logEntry}. Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs critical exceptions into a persistent file.
        /// This method executes in both Debug and Release configurations.
        /// </summary>
        /// <param name="ex">The exception object containing details and stack trace.</param>
        /// <param name="customMessage">An optional context message describing where or why the error occurred.</param>
        public static void LogException(Exception ex, string customMessage = "")
        {
            LogToEventLog("DVLD", $"Critical Exception: {customMessage}\nException Message: {ex.Message}\nStack Trace: {ex.StackTrace}", EventLogEntryType.Error);
            LogToFile(ex, customMessage);
        }
    }
}
