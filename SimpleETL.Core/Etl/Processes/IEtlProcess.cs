using System.Threading.Tasks;

namespace SimpleETL
{
    public interface IEtlProcess
    {
        void Run();
        Task RunAsync();
    }
}
