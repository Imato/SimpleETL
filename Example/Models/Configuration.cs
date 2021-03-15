using SimpleETL;
using System;
using System.Collections.Generic;

namespace DashboardETL.Models
{
    public class Configuration : EtlConfiguration
    {
        public static DateTime MinDate = DateTime.Parse("2018-01-01 00:00:00");
        public static string DefaultStringValue = "Unknown";
        public static int DefaultIdValue = -1;
        public static string DashboardDb = "DashboardDb";
        public Dictionary<string, SourceSettings> Sources { get; set; }
        public DateTime MaxDate => DateTime.Now;
    }
}
