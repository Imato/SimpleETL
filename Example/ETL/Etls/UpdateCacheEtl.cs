using DashboardETL.Models;
using SimpleETL;
using System;

namespace DashboardETL.ETL
{
    internal sealed class UpdateCacheEtl : DashboardEtlProcess
    {
        public UpdateCacheEtl() : base()
        {
        }

        protected override void Execute()
        {
            // new ExecProcedureOperation(this, "stg.UpdateEventsGroupsCache").Run();
            // new ExecProcedureOperation(this, "dim.UpdateDimValues").Run();
        }

        protected override void PreExecute()
        {
        }

        protected override void PostExecute()
        {
        }
    }
}
