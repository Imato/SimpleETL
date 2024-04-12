namespace Imato.SimpleETL
{
    public interface IEtlProcess : IEtlObject
    {
        void Run(CancellationToken token = default);

        public EtlState State { get; }
    }
}