using System;
using System.Collections.Generic;

namespace SimpleETL
{
    public interface IDataTransformation
    {
        IEnumerable<IEtlRow> TransformData(IEnumerable<IEtlRow> data);
    }
}
