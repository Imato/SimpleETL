using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleETL
{
    public interface IEtlPackage : IEtlProcess
    {
        void AddEtlProcess(IEtlProcess process);
    }
}
