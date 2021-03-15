using System;
using System.Collections.Generic;

namespace SimpleETL
{
    public abstract class DataTransformation : EtlObject, IDataTransformation, IDisposable
    {
        public abstract IEnumerable<IEtlRow> TransformData(IEnumerable<IEtlRow> data);
    }
}
