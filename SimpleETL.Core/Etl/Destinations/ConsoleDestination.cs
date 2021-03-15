using System;
using System.Collections.Generic;

namespace SimpleETL
{
    public class ConsoleDestination : DataDestination
    {
        public override void PutData(IEnumerable<IEtlRow> data)
        {
            foreach(var row in data)
            {
                PutData(row);
            }
        }
        public override void PutData(IEtlRow row)
        {
            Console.WriteLine(row);
        }
    }
}
