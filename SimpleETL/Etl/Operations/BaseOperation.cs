using System;
using System.Threading.Tasks;

namespace SimpleETL
{
    public class BaseOperation : EtlObject, IOperation
    {
        public virtual void Run()
        {
            Debug("Started");
        }

        public virtual Task RunAsync()
        {
            return Task.Run(() => Debug("Started Async"));
        }


    }
}
