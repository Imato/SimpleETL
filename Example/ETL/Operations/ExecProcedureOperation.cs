using SimpleETL;

namespace DashboardETL.ETL
{
    public class ExecProcedureOperation : SqlCommandOperation
    {
        private static string ConnectionString = Infrastructure.ConfigurationUtils.GetConnectionString();

        public ExecProcedureOperation(EtlObject parentEtl, string procedure)
            : base(ConnectionString, $"execute {procedure};", parentEtl, 240)
        {
        }
    }
}
