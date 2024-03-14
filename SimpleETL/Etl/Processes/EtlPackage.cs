using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleETL
{
    public class EtlPackage : EtlObject, IEtlPackage
    {
        private ConcurrentStack<IEtlProcess> _processes;

        public EtlPackage(string name) : base(name)
        {
            _processes = new ConcurrentStack<IEtlProcess>();
        }

        public EtlPackage():base()
        {
            _processes = new ConcurrentStack<IEtlProcess>();
        }

        public virtual void Run()
        {
            try
            {
                while (!_processes.IsEmpty)
                {
                    _processes.TryPop(out IEtlProcess p);
                    p.Run();
                }

                OnComplet?.Invoke(this);
            }

            catch
            {
                OnFailure?.Invoke(this);
                OnComplet?.Invoke(this);
                throw;
            }

            OnSuccess?.Invoke(this);
        }

        public virtual Task RunAsync()
        {
            try
            {
                var tasks = new List<Task>();

                while (!_processes.IsEmpty)
                {
                    _processes.TryPop(out IEtlProcess p);
                    tasks.Add(p.RunAsync());
                }

                Task.WaitAll(tasks.ToArray());

                OnComplet?.Invoke(this);
            }

            catch
            {
                OnFailure?.Invoke(this);
                OnComplet?.Invoke(this);
                throw;
            }

            OnSuccess?.Invoke(this);
            return Task.CompletedTask;
        }

        public void AddEtlProcess(IEtlProcess process)
        {
            var eo = process as EtlObject;
            if (eo != null)
            {
                eo.ParentEtl = this;
                _processes.Push(eo as IEtlProcess);
            }
            else
                _processes.Push(process);            
        }

        public EtlPackageEventHandler OnSuccess;
        public EtlPackageEventHandler OnFailure;
        public EtlPackageEventHandler OnComplet;
    }
}
