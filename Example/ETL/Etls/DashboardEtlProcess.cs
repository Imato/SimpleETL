using System;
using DashboardETL.Models;
using SimpleETL;

namespace DashboardETL.ETL
{
    public class DashboardEtlProcess : EtlProcess
    {
        public SourceSettings SourceSettings
        {
            get
            {
                if (Configuration.Sources != null
                    && Configuration.Sources.ContainsKey(Name))
                    return Configuration.Sources[Name];
                else
                    return null;
            }
        }
        public Configuration Configuration { get { return GetConfiguration<Configuration>(); } }

        public DashboardEtlProcess(string name) : base(name)
        {
            AddDefaultHandlers(this);
        }

        public DashboardEtlProcess() : base()
        {
        }

        protected override void PreExecute()
        {
            base.PreExecute();

            if (SourceSettings?.IsActive == true)
            {
                SourceSettings.LastStartDate = DateTime.Now;
            }

            if (SourceSettings?.IsActive == false)
                Log("Etl process and data source is not active in adm.Sources");
        }

        protected override void Execute()
        {
            if (SourceSettings?.IsActive == true)
                base.Execute();
        }

        public static void AddDefaultHandlers(EtlProcess etl)
        {
            etl.OnFailure += HandleError;
            etl.OnSuccess += HandleSucces;
        }

        private static void HandleError(EtlObject etl, EtlStatusEventArgs args)
        {
            var logger = EtlContext.GetContext().Logger;
            logger?.Error(etl, $"Error in ETL process!\n{args.ErrorMessage}");
        }

        private static void HandleSucces(EtlObject etl, EtlStatusEventArgs args)
        {
            var logger = EtlContext.GetContext().Logger;
            logger?.Information(etl, $"ETL process is succeed. Duration: {args.DurationSecconds:N3} seconds");
        }
    }
}
