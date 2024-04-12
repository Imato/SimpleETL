namespace Imato.SimpleETL
{
    public class DataDestination : EtlObject, IDataDestination
    {
        public virtual void PutData(IEnumerable<IEtlRow> data, CancellationToken token = default)
        {
            foreach (var row in data)
            {
                PutData(row);
            }
        }

        public virtual void PutData(IEtlRow row, CancellationToken token = default)
        {
            RowAffected++;
        }

        public int RowAffected { get; protected set; }
    }
}