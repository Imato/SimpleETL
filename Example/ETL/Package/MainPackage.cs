using SimpleETL;
using System.Threading.Tasks;

namespace DashboardETL.ETL
{
    public class MainPackage : EtlPackage
    {
        // Pre process - update dictionaries 
        private readonly DictionaryPackage dics = new DictionaryPackage();
        // Load data from sources
        private readonly FactPackage facts = new FactPackage();
        // Post process - update dictionaries
        private readonly PostProcessPackage post = new PostProcessPackage();

        private readonly UpdateCacheEtl updateCache = new UpdateCacheEtl();

        public MainPackage()
        {
            dics.OnSuccess += async (_) =>
            {
                Log("DictionaryPackage: OnSuccess");
                await facts.RunAsync();
            };

            facts.OnComplet += async (_) =>
            {
                Log("FactPackage: OnComplet");
                await post.RunAsync();
            };

            post.OnComplet += async (_) =>
            {                
                await updateCache.RunAsync();
                Log("Package is completed");
            };
        }        

        public override async Task RunAsync()
        {
            await dics.RunAsync();
        }
    }
}
