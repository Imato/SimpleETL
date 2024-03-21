namespace Imato.SimpleETL
{
    public interface IEtlPackage : IEtlProcess
    {
        void AddEtlProcess(IEtlProcess process);
    }
}