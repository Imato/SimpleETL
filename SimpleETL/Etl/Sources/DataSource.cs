using System;
using System.Collections.Generic;

namespace Imato.SimpleETL
{
    public abstract class DataSource : EtlObject, IDataSource
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

        public abstract IEnumerable<IEtlRow> GetData();

        public int RowAffected { get; protected set; }
        public DateTime LastDate { get; protected set; }
    }
}