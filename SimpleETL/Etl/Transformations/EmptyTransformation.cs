using System.Collections.Generic;

namespace SimpleETL
{
    public class EmptyTransformation : DataTransformation
    {

        public override IEnumerable<IEtlRow> TransformData(IEnumerable<IEtlRow> data)
        {
            return data;
        }
    }
}
