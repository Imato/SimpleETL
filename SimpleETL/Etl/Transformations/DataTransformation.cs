namespace Imato.SimpleETL
{
    public class DataTransformation : EtlObject, IDataTransformation
    {
        public virtual IEnumerable<IEtlRow> TransformData(IEnumerable<IEtlRow> data)
        {
            foreach (var row in data)
            {
                yield return TransformData(row);
            }
        }

        public virtual IEtlRow TransformData(IEtlRow row)
        {
            return row;
        }
    }
}