namespace Imato.SimpleETL
{
    public class BaseOperation : EtlObject, IOperation
    {
        public virtual void Run()
        {
            Debug("Started");
        }
    }
}