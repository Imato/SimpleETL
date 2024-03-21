using System;
using System.Collections.Generic;

namespace Imato.SimpleETL
{
    public interface IDataTransformation
    {
        IEnumerable<IEtlRow> TransformData(IEnumerable<IEtlRow> data);
    }
}
