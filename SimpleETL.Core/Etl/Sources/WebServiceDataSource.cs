using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SimpleETL
{
    public class WebServiceDataSource : DataSource
    {
        private readonly string _jsonPath;
        private readonly Type _dataType;
        private readonly string _url;
        private readonly HttpClientHandler _handler;
        private readonly int _timeOutSec;

        public WebServiceDataSource(string url, 
            string jsonPath, 
            Type dataType, 
            HttpClientHandler handler = null,
            EtlObject parent = null,
            int timeOutSec = 60)
        {
            Debug($"Create WebServiceDataSource for {url}");

            _url = url;
            _jsonPath = jsonPath;
            _dataType = dataType;
            _handler = handler ?? new HttpClientHandler();
            ParentEtl = parent;
            _timeOutSec = timeOutSec;
        }

        protected IEnumerable<IEtlRow> GetData(Type type)
        {
            Log($"Get data from WEB {_url}");
            var rows = 0;

            using (_handler)
            {
                using (var http = new HttpClient(_handler))
                {
                    http.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    if(_handler?.Credentials != null)
                    {
                        var c = (NetworkCredential) _handler.Credentials;
                        var byteArray = new UTF8Encoding().GetBytes($"{c.UserName}:{c.Password}");
                        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    http.Timeout = TimeSpan.FromSeconds(_timeOutSec);

                    var responce = http.GetAsync(_url).Result;
                    var content = responce.Content.ReadAsStringAsync().Result;

                    if (!responce.IsSuccessStatusCode)
                    {
                        Error($"StatusCode: {responce.StatusCode }");
                        Error($"Content: {content}");
                    }
                    else
                    {
                        Debug(content);
                    }

                    var jt = JToken.Parse(content);

                    foreach (var row in jt.GetRows(type, _jsonPath, flow))
                    {
                        rows++;
                        yield return row;
                    }

                    Log($"Return {rows} rows from WEB {_url}");
                }
            }
        }

        public override IEnumerable<IEtlRow> GetData()
        {
            return GetData(_dataType);
        }
    }
}
