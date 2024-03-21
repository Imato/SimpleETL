using System.Collections.Generic;

namespace Imato.SimpleETL
{
    public interface IDataDestination
    {
        void PutData(IEnumerable<IEtlRow> data);

        void PutData(IEtlRow row);
    }
}