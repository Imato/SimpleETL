using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Imato.SimpleETL
{
    public class WebServiceDataSource : DataSource
    {
        private readonly string? _jsonPath;
        private readonly Type? _dataType;
        private readonly string _url;
        private readonly HttpClient client = null!;

        public WebServiceDataSource(string url,
            string? jsonPath = null,
            Type? dataType = null,
            HttpClientHandler? handler = null,
            EtlObject? parent = null,
            int timeOutSec = 60,
            IEnumerable<Tuple<string, string>>? headers = null)
        {
            Debug($"Create WebServiceDataSource for {url}");

            _url = url;
            _jsonPath = jsonPath;
            _dataType = dataType;
            ParentEtl = parent;

            if (client == null)
            {
                client = handler != null ? new HttpClient(handler) : new HttpClient();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                if (headers != null && headers.Any())
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(
                            header.Item1,
                            header.Item2);
                    }
                }
                if (handler?.Credentials != null)
                {
                    var c = (NetworkCredential)handler.Credentials;
                    var byteArray = new UTF8Encoding().GetBytes($"{c.UserName}:{c.Password}");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                }
                client.Timeout = TimeSpan.FromSeconds(timeOutSec);
            }
        }

        protected IEnumerable<IEtlRow> GetData(Type type, CancellationToken token = default)
        {
            Debug($"Get data from WEB {_url}");
            var rows = 0;
            var responce = client.GetAsync(_url, token).Result;
            var content = responce.Content.ReadAsStringAsync(token).Result;

            if (!responce.IsSuccessStatusCode)
            {
                Error($"StatusCode: {responce.StatusCode}");
                Error($"Content: {content}");
            }
            else
            {
                Debug(content);
            }

            if (!string.IsNullOrEmpty(content) && content != "[]")
            {
                var jt = JToken.Parse(content);

                foreach (var row in jt.GetRows(type, _jsonPath, Flow))
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    rows++;
                    yield return row;
                }
            }

            Debug($"Return {rows} rows from WEB {_url}");
        }

        public override IEnumerable<IEtlRow> GetData(CancellationToken token = default)
        {
            return GetData(_dataType);
        }
    }
}