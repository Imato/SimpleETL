namespace Imato.SimpleETL
{
    public class DataDestination : EtlObject, IDataDestination
    {
        protected IEtlDataFlow Flow = null!;

        public virtual void PutData(IEnumerable<IEtlRow> data, CancellationToken token = default)
        {
            foreach (var row in data)
            {
                PutData(row);
            }
        }

        public virtual void PutData(IEtlRow row, CancellationToken token = default)
        {
            if (RowsAffected == 0)
            {
                Flow = row.Flow;
            }
            RowsAffected++;
        }

        public int RowsAffected { get; set; }
    }
}