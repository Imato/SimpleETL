namespace Imato.SimpleETL
{
    public interface IDataTransformation
    {
        IEnumerable<IEtlRow> TransformData(IEnumerable<IEtlRow> data);

        IEtlRow TransformData(IEtlRow data);
    }
}