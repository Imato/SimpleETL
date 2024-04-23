namespace Imato.SimpleETL
{
    public class DataSource : EtlObject, IDataSource
    {
        protected IEtlDataFlow Flow = new EtlDataFlow();

        protected IEtlRow CreateRow()
        {
            return new EtlRow(Flow);
        }

        public void AddColumn(EtlColumn column)
        {
            Flow.AddColumn(column);
        }

        public virtual IEnumerable<IEtlRow> GetData(CancellationToken token = default)
        {
            RowsAffected = 0;
            LastDate = DateTime.Now;
            return Enumerable.Empty<IEtlRow>();
        }

        public int RowsAffected { get; protected set; }
        public DateTime LastDate { get; protected set; }
    }
}