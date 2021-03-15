using DashboardETL.Infrastructure;
using DashboardETL.Models;
using SimpleETL;
using System;
using System.Linq;

namespace DashboardETL.ETL
{
    internal sealed class CommonEtl : DashboardEtlProcess
    {
        private static string _name = "Common";

        public CommonEtl() : base(_name)
        {

        }

        protected override void PreExecute()
        {
            base.PreExecute();

            if (Configuration.Sources != null)
            {
                // var so = new SaveSourceOperation(this);

                foreach (var s in Configuration.Sources.Values.Where(x => x.SourceType == _name))
                {
                    s.LastStartDate = DateTime.Now;
                    // so.SaveLastStart(s);
                }
            }
        }

        protected override void Execute()
        {
            var procedure = new SqlCommandOperation(Infrastructure.ConfigurationUtils.GetConnectionString(),
                "exec stg.LoadCommonEventStore",
                this);
            procedure.Run();

            base.Execute();
        }
    }
}
