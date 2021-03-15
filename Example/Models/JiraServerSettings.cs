using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardETL.Models
{
    public class JiraServerSettings
    {
        public string Name
        {
            get
            {
                if (ApiUrl == null)
                    return "Unknown";

                return new Uri(ApiUrl).Host;
            }
        }        
        public string ApiUrl { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ConsumerKey { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }        

        public override string ToString()
        {
            return Name;
        }
    }
}
