namespace Imato.SimpleETL
{
    public class EtlStatusEventArgs : EventArgs
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double DurationSeconds { get; set; }
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; } = "";
    }

    public delegate void EtlEventHandler(EtlObject sender, EtlStatusEventArgs e);

    public delegate void EtlPackageEventHandler(EtlObject sender);
}