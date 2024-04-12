namespace Imato.SimpleETL
{
    public class EtlState : EtlStatusEventArgs
    {
        public bool IsActive { get; set; } = true;
    }
}