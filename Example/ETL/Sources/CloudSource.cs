using DashboardETL.Infrastructure;
using DashboardETL.Models;
using SimpleETL;
using System;
using System.Collections.Generic;

namespace DashboardETL.ETL
{
    public class CloudSource : DataSource 
    {
        private static string _jsonPath = "";
        private static Type _dataType = typeof(string);
        private readonly WebServiceDataSource _source;

        public CloudSource(Cloud cloud, DateTime lastProcessDate, EtlProcess parent)
        {
            Name = cloud.Name;
            
            _source = new WebServiceDataSource(string.Format(cloud.Url, GetTimeInterval(lastProcessDate)), 
                _jsonPath, 
                _dataType, 
                HttpUtils.GetDefaultHandler(),
                this,
                180);

            LastDate = lastProcessDate;
            ParentEtl = parent;
        }

        public override IEnumerable<IEtlRow> GetData()
        {
            foreach (var row in _source.GetData())
            {
                row["CloudName"] = Name;
                yield return row;
            }
        }

        public void UpdateLastDate(DateTime lastDate)
        {
            if (lastDate > LastDate)
                LastDate = lastDate;
        }

        private string GetTimeInterval(DateTime date)
        {
            var now = DateTime.Now;

            int minutes = (int) Math.Round((now - date).TotalMinutes, 0);
            if (minutes == 0)
                return "1m";

            return $"{minutes}m";

            /* Use determent minutes interval!
            
            if (minutes > 0 && minutes <= 30)
                return $"{minutes}m";

            int hours = (int)Math.Round((now - date).TotalHours, 0);
            if (hours > 0 && hours <= 24)
                return $"{hours}h";

            int days = (int)Math.Round((now - date).TotalDays, 0);
            return $"{days}d";
            */
        }
    }
}
