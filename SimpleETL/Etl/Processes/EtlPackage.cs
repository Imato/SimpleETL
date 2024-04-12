namespace Imato.SimpleETL
{
    public class EtlPackage : EtlProcess, IEtlPackage
    {
        private Queue<IEtlProcess> _processes;

        public EtlPackage(string name) : this()
        {
            Name = name;
        }

        public EtlPackage()
        {
            _processes = new Queue<IEtlProcess>();
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

                while (_processes.Count > 0)
                {
                    if (_processes.TryDequeue(out IEtlProcess p))
                    {
                        tasks.Add(Task.Factory.StartNew(
                            () =>
                            {
                                p.Run();
                                p.Dispose();
                            }, token));
                    }
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
            _processes.Enqueue(process);

            EtlContext.Register(process);
        }
    }
}