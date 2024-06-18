namespace Imato.SimpleETL
{
    public interface IEtlPackage : IEtlProcess
    {
        void AddEtlProcess(Func<IEtlProcess> processFactory);
    }
}