using DashboardETL.Models;
using SimpleETL;
using System;

namespace DashboardETL.ETL
{
    internal sealed class SmEtl : DashboardEtlProcess
    {
        private static string name = "SM";
        private DateTime startDate;

        public SmEtl() : base(name)
        {
            if (SourceSettings.IsActive)
            {
                startDate = DateTime.Now;

                var dateFrom = SourceSettings.LastProcessDate;

                Source = new SqlDataSource(SourceSettings.SqlConnetctionString,
                    SourceSettings.SqlQuery.Replace("@dateFrom", $"'{dateFrom.ToString(DateTimeUtils.SqlDateFormat)}'"));

                Destination = new SqlBulkInsertDestination(Infrastructure.ConfigurationUtils.GetConnectionString(),
                        SourceSettings.DestinationTable,
                        100000,
                        "DateId,SourceName,EventName,ActionName,EventValue".Split(','),
                        this);

                Transformation = new SmTransformation(this);
            }
        }

        protected override void PreExecute()
        {
            if (SourceSettings?.IsActive == true)
            {
                SourceSettings.LastStartDate = DateTime.Now;
            }
        }

        protected override void PostExecute()
        {
            if (SourceSettings?.IsActive == true)
            {
                var lastDate = ((SmTransformation)Transformation).LastDate;
                if (SourceSettings.LastProcessDate < lastDate)
                    SourceSettings.LastProcessDate = lastDate;
            }

            base.PostExecute();
        }

    }
}
