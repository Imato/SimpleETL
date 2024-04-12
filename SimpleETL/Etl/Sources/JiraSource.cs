using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Imato.SimpleETL
{
    public class JiraSource : DataSource
    {
        private readonly HttpClient _http;
        private readonly string _url;
        private readonly List<JiraIssueField> _fields;
        private readonly string _jql;
        private readonly IEnumerable<string> _taskList;

        public static string JiraTimeFormat = @"yyyy\/MM\/dd HH:mm";

        /// <summary>
        /// Create JIRA source
        /// </summary>
        /// <param name="url">JIRA API URL</param>
        /// <param name="basicCredentials">Credentials: user:password</param>
        /// <param name="fields">List of JIRA fields: key:jiraField.property value:dataFlowField or for strings (not jira objects) key:jiraField value:dataFlowField</param>
        /// <param name="jql">JQL for select data</param>
        /// <param name="taskList">OR JIRA tasks list for select data</param>
        public JiraSource(string url, string basicCredentials,
            IEnumerable<KeyValuePair<string, string>> fields,
            string jql = null,
            IEnumerable<string> taskList = null,
            EtlObject parent = null)
        {
            if (jql == null && taskList == null)
                throw new ArgumentException($"Use {nameof(jql)} or {nameof(taskList)} in JIRA source");

            _jql = jql;
            _taskList = taskList;
            _url = url ?? throw new ArgumentNullException(nameof(url));
            ParentEtl = parent;

            if (fields == null)
                throw new ArgumentNullException(nameof(fields));
            else
            {
                _fields = new List<JiraIssueField>();

                foreach (var f in fields.OrderBy(x => x.Key))
                {
                    var jf = new JiraIssueField
                    {
                        DbField = f.Value,
                        Key = f.Key.Contains(".") ? f.Key.Split(".")[0] : f.Key,
                        SubKey = f.Key.Contains(".") ? f.Key.Split(".")[1] : null
                    };

                    _fields.Add(jf);
                }
            }

            _http = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                        Encoding.Default.GetBytes(basicCredentials)));
        }

        public override IEnumerable<IEtlRow> GetData(CancellationToken token = default)
        {
            Debug($"Try to get data from JIRA: {_url}");

            var rowCount = 0;

            foreach (var issue in GetIssues())
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                rowCount++;
                yield return GetNewRow(issue);
            }

            Debug($"Total {rowCount} rows processed");
        }

        private IEnumerable<JToken> GetIssues()
        {
            if (_taskList == null)
            {
                var uri = new Uri($"{_url}/search?jql={_jql}");

                foreach (var item in GetIssues(uri))
                {
                    yield return item;
                }
            }
            else
                foreach (var task in _taskList)
                {
                    var taskUri = new Uri($"{_url}/issue/{task}");
                    foreach (var item in GetIssues(taskUri))
                    {
                        yield return item;
                    }
                }
        }

        private IEnumerable<JToken> GetIssues(Uri uri)
        {
            Debug($"Get date from {uri}");

            var responce = _http.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;
            var json = JsonConvert.DeserializeObject<JObject>(responce);

            var error = json["errorMessages"];
            if (error != null)
            {
                var errorText = error[0].Value<string>();

                // Ignore not exists
                if (errorText.Contains("Does Not Exist")
                    || errorText.Contains("do not have the permission"))
                {
                    Debug($"{uri}: errorText");
                    yield break;
                }

                throw new ApplicationException(errorText);
            }

            if (json["issues"] != null)
            {
                foreach (var t in json["issues"].ToList())
                {
                    yield return t;
                }
            }
            else
            {
                yield return json;
            }
        }

        private IEtlRow GetNewRow(JToken issue)
        {
            var row = CreateRow();

            if (issue.HasValues)
            {
                row["key"] = issue["key"]?.Value<string>();

                foreach (var field in _fields)
                {
                    JToken value = null;

                    if (field.SubKey == null)
                        value = issue["fields"][field.Key];

                    if (field.SubKey != null
                        && issue["fields"][field.Key] != null
                        && issue["fields"][field.Key].HasValues)
                        value = issue["fields"][field.Key][field.SubKey];

                    row[field.DbField] = value?.GetTypedValue();
                }
            }

            return row;
        }
    }

    public class JiraIssueField
    {
        public string Key { get; set; }
        public string SubKey { get; set; }
        public string DbField { get; set; }
    }
}