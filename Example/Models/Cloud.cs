using Newtonsoft.Json;
using System;

namespace DashboardETL.Models
{
    public class Cloud
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime LastDate { get; set; }

        [JsonIgnore]
        public string AdminApiUrl => $"https://cloud.admin.{Name}.odkl.ru/api/clouds/{Name}/web";

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
