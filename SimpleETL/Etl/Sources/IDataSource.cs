using System.Collections.Generic;

namespace SimpleETL
{
    public interface IDataSource
    {
        IEnumerable<IEtlRow> GetData();

    }
}
