namespace Imato.SimpleETL
{
    public class EtlPackage : EtlProcess, IEtlPackage
    {
        private readonly List<IEtlProcess> _processes;

        public EtlPackage(string name) : this()
        {
            Name = name;
        }

        public EtlPackage()
        {
            _processes = new();
        }

        public override void Run(CancellationToken token = default)
        {
            if (!State.IsActive)
            {
                Debug("Process IsActive = false");
                return;
            }

            var tasks = new List<Task>();

            try
            {
                State.StartTime = DateTime.Now;
                OnStart?.Invoke(this, State);

                PreExecute();

                foreach (var p in _processes)
                {
                    tasks.Add(Task.Factory.StartNew(
                            () =>
                            {
                                p.Run();
                            }, token));
                }

                Task.WaitAll(tasks.ToArray());

                PostExecute();

                State.IsSuccessful = true;
                OnSuccess?.Invoke(this, State);
                Finish();
            }
            catch (Exception e)
            {
                State.IsSuccessful = false;
                State.ErrorMessage = e.ToLogString();
                Finish();

                OnFailure?.Invoke(this, State);
                OnComplete?.Invoke(this, State);
                throw;
            }

            OnFinish?.Invoke(this, State);
            OnComplete?.Invoke(this, State);
        }

        public void AddEtlProcess(IEtlProcess process)
        {
            process.ParentEtl = this;
            _processes.Add(process);

            EtlContext.Register(process);
        }

        public override void Dispose()
        {
            foreach (var process in _processes)
            {
                process.Dispose();
            }
            base.Dispose();
        }
    }
}