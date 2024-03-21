using System.Collections.Generic;

namespace Imato.SimpleETL
{
    public interface IDataSource
    {
        IEnumerable<IEtlRow> GetData();

    }
}
