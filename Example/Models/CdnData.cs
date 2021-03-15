using System;

namespace DashboardETL.Models
{
    public class CdnData
    {
        public string UserName { get; set; }
        public DateTime CommitDate { get; set; }
        public string CommitComments { get; set; }
        public string DataSourceLabel { get; set; }
    }
}
