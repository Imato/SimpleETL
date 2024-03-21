namespace Imato.SimpleETL
{
    public interface IRowOperation
    {
        IEtlRow Process(IEtlRow row);
    }
}