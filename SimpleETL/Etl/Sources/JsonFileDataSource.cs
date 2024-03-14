using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SimpleETL
{
    public class JsonFileDataSource : DataSource
    {
        private string _filesFolder;
        private string _jsonPath;
        private Type _dataType;
        private DateTime _lastDate;

        public DateTime LastFileDate { get; private set; }
        public string LastFileName { get; private set; }

        public JsonFileDataSource(
            string filesFolder,
            string jsonPath,
            DateTime lastDate,
            Type dataType)
        {
            _filesFolder = filesFolder;
            _lastDate = lastDate;
            _dataType = dataType;
            _jsonPath = jsonPath;
        }

        private IEnumerable<IEtlRow> GetData(Type type)
        {
            Log($"Get JSON files data source from folder {_filesFolder}");

            if (!Directory.Exists(_filesFolder))
                Error($"Directory {_filesFolder} not exists!");
            else
            {
                var files = Directory.EnumerateFiles(_filesFolder, "*.*", SearchOption.AllDirectories)
                    .Where(f => File.GetLastWriteTime(f) > _lastDate)
                    .OrderBy(f => File.GetLastWriteTime(f));

                var rows = 0;

                foreach (var file in files)
                {
                    Log($"Open file {file}");

                    var content = File.ReadAllText(file);
                    var jt = JToken.Parse(content);

                    foreach (var row in jt.GetRows(type, _jsonPath, flow))
                    {
                        rows++;
                        yield return row;
                    }

                    Log($"Return {rows} rows from {file}");

                    LastFileName = file;
                    LastFileDate = File.GetLastWriteTime(file);
                    rows = 0;
                }
            }
        }

        public override IEnumerable<IEtlRow> GetData()
        {
            return GetData(_dataType);
        }
    }
}