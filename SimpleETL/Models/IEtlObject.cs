namespace Imato.SimpleETL
{
    public interface IEtlObject : IDisposable
    {
        string Name { get; set; }
        public EtlObject? ParentEtl { get; set; }
    }
}