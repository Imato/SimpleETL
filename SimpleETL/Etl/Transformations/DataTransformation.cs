namespace Imato.SimpleETL
{
    public class DataTransformation : EtlObject, IDataTransformation
    {
        public virtual IEnumerable<IEtlRow> TransformData(IEnumerable<IEtlRow> data)
        {
            return data;
        }
    }
}