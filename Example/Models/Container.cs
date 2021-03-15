using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardETL.Models
{
    public class Container
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CloudName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceSimpleName { get; set; }
        public string QueueName { get; set; }
        [JsonProperty("image")]
        public string ImageName { get; set; }
        [JsonProperty("minion")]
        public string MinionName { get; set; }
        [JsonProperty("container")]
        public string BaseContainerName { get; set; }
        public bool IsExists => ServiceName != null;

        public override string ToString()
        {
            return $"Container: {Name}, Service: {ServiceName}";
        }

    }
}
