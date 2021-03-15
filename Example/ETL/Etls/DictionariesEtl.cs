using DashboardETL.Models;
using SimpleETL;

namespace DashboardETL.ETL
{
    public class DictionariesEtl : DashboardEtlProcess
    {
        private static string _name = "Dictionaries";

        public DictionariesEtl() : base(_name)
        {
        }

        protected override void Execute()
        {
            // Not used now
            // Log("Update dim.Dates");
            // new SqlCommandOperation(Configuration.DashboardDd.SqlConnectionString, "exec stg.AddDates").Run();
        }
    }
}
