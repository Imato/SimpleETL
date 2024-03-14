using System;
using System.Collections.Generic;

namespace SimpleETL
{
    public abstract class DataDestination : EtlObject, IDataDestination
    {

        public DataDestination()
        {
            Debug("Open data destination");
        }


        public override void Dispose()
        {
            Debug("Close data destination");
        }

        public abstract void PutData(IEnumerable<IEtlRow> data);
        public abstract void PutData(IEtlRow row);

        public int RowAffected { get; protected set; }
    }
}
