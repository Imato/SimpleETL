using SimpleETL;

namespace DashboardETL.ETL
{
    public class FactPackage : EtlPackage
    {
        public FactPackage()
        {
            AddEtlProcess(new SmEtl());
        }
    }
}
