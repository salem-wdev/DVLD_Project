using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Shared
{
    public static class clsLogger
    {
        public enum LogLevel { Info, Warning, Error }

        [Conditional("DEBUG")]
        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            string formattedMessage = $"[{level.ToString().ToUpper()}] {DateTime.Now}: {message}";
            System.Diagnostics.Debug.WriteLine(formattedMessage);
        }
    }
}
