using System.Threading.Tasks;

namespace SimpleETL
{
    public interface IOperation
    {
        void Run();
        Task RunAsync();
    }
}
