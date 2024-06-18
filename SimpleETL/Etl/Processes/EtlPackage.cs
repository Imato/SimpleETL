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
                    tasks.Add(Task.Factory.StartNew(() => p.Run(), token));
                }

                Task.WaitAll(tasks.ToArray());

                PostExecute();

                State.IsSuccessful = true;
                State.ErrorMessage = string.Empty;

                OnSuccess?.Invoke(this, State);
            }
            catch (Exception e)
            {
                State.IsSuccessful = false;
                State.ErrorMessage = e.ToLogString();

                OnFailure?.Invoke(this, State);
                OnComplete?.Invoke(this, State);
                throw;
            }

            Finish();
            OnFinish?.Invoke(this, State);
            OnComplete?.Invoke(this, State);
        }

        public void AddEtlProcess(Func<IEtlProcess> processFactory)
        {
            if (processFactory != null)
            {
                try
                {
                    var process = processFactory();
                    process.ParentEtl = this;
                    _processes.Add(process);
                    EtlContext.Register(process);
                }
                catch (Exception e)
                {
                    Error("Cannot create ETL process", e);
                }
            }
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