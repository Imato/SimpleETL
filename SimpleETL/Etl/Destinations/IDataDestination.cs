using System;
using System.Collections.Generic;

namespace SimpleETL
{
    public interface IDataDestination
    {
        void PutData(IEnumerable<IEtlRow> data);        
    }
}
