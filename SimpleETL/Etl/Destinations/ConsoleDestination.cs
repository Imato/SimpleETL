using System;

namespace Imato.SimpleETL
{
    public class ConsoleDestination : DataDestination
    {
        public override void PutData(IEtlRow row, CancellationToken token = default)
        {
            Console.WriteLine(row);
        }
    }
}