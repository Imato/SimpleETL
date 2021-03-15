using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleETL;
using System.IO;
using Newtonsoft.Json;

namespace SimpleETL.Test.Etl.Sources
{

    [TestClass]
    public class WebServiceDataSourceTests : WebServiceDataSource
    {

        public WebServiceDataSourceTests() : base(null, null, typeof(Container), null)
        {
        }



        [TestMethod]
        public void TestGetDataTyped()
        {
            var data = File.ReadAllText("./Etl/Sources/web-service-data-container-1.json");
            // var rows = GetData<Container>(data);
            // Assert.AreEqual(1, rows.Count());
            
        }

    }

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

    }
}
