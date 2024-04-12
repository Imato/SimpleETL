namespace Imato.SimpleETL
{
    public class DataSource : EtlObject, IDataSource
    {
        protected IEtlDataFlow flow;

        public DataSource() : base()
        {
            flow = new EtlDataFlow();
        }

        protected IEtlRow CreateRow()
        {
            return new EtlRow(flow);
        }

        public virtual IEnumerable<IEtlRow> GetData(CancellationToken token = default)
        {
            RowAffected = 0;
            LastDate = DateTime.Now;
            return Enumerable.Empty<IEtlRow>();
        }

        public int RowAffected { get; protected set; }
        public DateTime LastDate { get; protected set; }
    }
}