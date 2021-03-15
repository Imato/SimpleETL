using DashboardETL.Models;
using SimpleETL;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashboardETL.ETL
{
    internal sealed class SmTransformation : DataTransformation
    {
        public DateTime LastDate { get; private set;  }

        public SmTransformation(EtlProcess parent)
        {
            ParentEtl = parent;
        }

        public override IEnumerable<IEtlRow> TransformData(IEnumerable<IEtlRow> data)
        {
            foreach (var row in data)
            {
                var date = (DateTime)row["dt"];
                if (LastDate < date) LastDate = date;
                row["DateId"] = date.GetDateId();
                row["SourceName"] = ParentEtl.Name;
                row["EventName"] = row["checkName"];
                row["ActionName"] = "PUT";
                row["EventValue"] = row["anomalies"];

                yield return row;
            }
        }
    }
}
