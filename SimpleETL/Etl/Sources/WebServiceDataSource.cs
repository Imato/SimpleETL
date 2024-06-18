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
        private readonly HttpClientHandler _handler;
        private readonly int _timeOutSec;
        private readonly IEnumerable<Tuple<string, string>>? _headers;

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
            _handler = handler ?? new HttpClientHandler();
            ParentEtl = parent;
            _timeOutSec = timeOutSec;
            _headers = headers;
        }

        protected IEnumerable<IEtlRow> GetData(Type type, CancellationToken token = default)
        {
            Debug($"Get data from WEB {_url}");
            var rows = 0;

            using (_handler)
            {
                using (var http = new HttpClient(_handler))
                {
                    http.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    if (_headers != null && _headers.Any())
                    {
                        foreach (var header in _headers)
                        {
                            http.DefaultRequestHeaders.TryAddWithoutValidation(
                                header.Item1,
                                header.Item2);
                        }
                    }
                    if (_handler?.Credentials != null)
                    {
                        var c = (NetworkCredential)_handler.Credentials;
                        var byteArray = new UTF8Encoding().GetBytes($"{c.UserName}:{c.Password}");
                        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    http.Timeout = TimeSpan.FromSeconds(_timeOutSec);

                    var responce = http.GetAsync(_url, token).Result;
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
            }
        }

        public override IEnumerable<IEtlRow> GetData(CancellationToken token = default)
        {
            return GetData(_dataType);
        }
    }
}