using System;
using System.Collections.Generic;

namespace SimpleETL
{
    public abstract class DataSource : EtlObject, IDataSource
    {
        protected IEtlDataFlow flow;

        public DataSource()
        {
            Debug("Open data source");
            flow = new EtlDataFlow();
        }
        

        public override void Dispose()
        {
            Debug("Close data source");
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
