using SimpleETL.Models;
using System;
using System.Collections.Generic;

namespace SimpleETL
{
    public class EtlConfiguration
    {
        public Dictionary<string, string> ConnectionStrings { get; set; }
        public Logging Logging { get; set; }
        public string LogFileFolder { get; set; }
        public int LogFilesHistoryDays { get; set; }

        public LogLevel GetLogLevel()
        { 
            if (Logging.LogLevel != null 
                && Logging.LogLevel.ContainsKey("Default"))
            {
                return (LogLevel)Enum.Parse(typeof(LogLevel), Logging.LogLevel["Default"]);
            }

            return LogLevel.Information;
        }
    }
}
