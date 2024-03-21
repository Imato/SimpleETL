using System.Collections.Generic;

namespace Imato.SimpleETL
{
    public abstract class DataDestination : EtlObject, IDataDestination
    {
        public virtual void PutData(IEnumerable<IEtlRow> data)
        {
            foreach (var row in data)
            {
                PutData(row);
            }
        }

        public abstract void PutData(IEtlRow row);

        public int RowAffected { get; protected set; }
    }
}