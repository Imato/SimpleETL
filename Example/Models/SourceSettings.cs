using System;

namespace DashboardETL.Models
{
    public class SourceSettings
    {
        public Int16 Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string SourceType { get; set; }
        public DateTime LastProcessDate { get; set; }
        public string SqlConnetctionString { get; set; }
        public string SqlQuery { get; set; }
        public string SourceFolder { get; set; }
        public string JiraUrl { get; set; }
        public string JiraCredentials { get; set; }
        public string JiraFields { get; set; }
        public string JiraJql { get; set; }
        public string DestinationTable { get; set; }
        public string WebServiceUrl { get; set; }
        public string KafkaBrockersList { get; set; }
        public string KafkaTopicsList { get; set; }
        public string Credentials { get; set; }
        public DateTime LastStartDate { get; set; }


        public override string ToString()
        {
            return Name;
        }
    }
}
