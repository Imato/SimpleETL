using System.Collections.Concurrent;

namespace Imato.SimpleETL
{
    public class EtlPackage : EtlObject, IEtlPackage
    {
        private ConcurrentStack<IEtlProcess> _processes;

        public EtlPackage(string name) : this()
        {
            Name = name;
        }

        public EtlPackage()
        {
            _processes = new ConcurrentStack<IEtlProcess>();
        }

        public virtual void Run()
        {
            var tasks = new List<Task>();

            try
            {
                while (!_processes.IsEmpty)
                {
                    _processes.TryPop(out IEtlProcess p);
                    tasks.Add(Task.Factory.StartNew(() => p.Run()));
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