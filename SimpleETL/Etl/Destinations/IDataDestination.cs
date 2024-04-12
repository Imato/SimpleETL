namespace Imato.SimpleETL
{
    public interface IDataDestination
    {
        void PutData(IEnumerable<IEtlRow> data, CancellationToken token = default);

        void PutData(IEtlRow row, CancellationToken token = default);
    }
}