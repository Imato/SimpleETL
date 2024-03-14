using System;
using System.Linq;
using System.Collections.Generic;

namespace SimpleETL
{
    public class HadoopHistoryDataSource : DataSource
    {
        private static DateTime _unix = new DateTime(1970, 1, 1);
        private static string _dateFormat = "yyyy-MM-dd HH:mm:ss";

        private readonly string _jsonPath;
        private readonly Type _dataType;
        private readonly DateTime _lastDate;
        private readonly string _url;

        public HadoopHistoryDataSource(string url, string jsonPath, DateTime lastDate, Type dataType) : base()
        {
            _url = url;
            _jsonPath = jsonPath;
            _lastDate = lastDate;
            _dataType = dataType;
        }

        public override IEnumerable<IEtlRow> GetData()
        {
            var dateFrom = _lastDate;
            var dateNow = DateTime.Parse(DateTime.Now.ToString(_dateFormat));
            var dateTo = dateNow;

            Log($"Get data from Hadoop history for {dateFrom.ToString(_dateFormat)} - {dateTo.ToString(_dateFormat)} ");

            var urlStart = $"{_url}?startedTimeBegin={(dateFrom - _unix).TotalMilliseconds}&startedTimeEnd={(dateTo - _unix).TotalMilliseconds}";
            var urlFinish = $"{_url}?finishedTimeBegin={(dateFrom - _unix).TotalMilliseconds}&finishedTimeEnd={(dateTo - _unix).TotalMilliseconds}";

            using (var sourceStart = new WebServiceDataSource(urlStart, _jsonPath, _dataType, null, this))
            {
                using (var sourceFinish = new WebServiceDataSource(urlFinish, _jsonPath, _dataType, null, this))
                {
                    LastDate = dateTo;
                    return sourceStart.GetData()
                        .Union(sourceFinish.GetData());
                }                    
            }            
        }
    }
}
