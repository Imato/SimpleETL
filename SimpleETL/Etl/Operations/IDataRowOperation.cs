using System.Collections.Generic;

namespace SimpleETL
{
    public interface IDataRowOperation
    {
        IEtlRow Process(IEtlRow row = null);
    }
}
