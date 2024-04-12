namespace Imato.SimpleETL
{
    public class EtlProcess : EtlObject, IEtlProcess
    {
        private DataSource _source = new();
        private DataTransformation _transformation = new();
        private DataDestination _destination = new();

        public EtlEventHandler OnStart = null!;
        public EtlEventHandler OnFinish = null!;
        public EtlEventHandler OnSuccess = null!;
        public EtlEventHandler OnFailure = null!;
        public EtlEventHandler OnComplete = null!;

        public EtlState State { get; }

        public EtlProcess()
        {
            OnStart += (_, args) => Debug($"Process started at {args.StartTime}");
            OnFinish += (_, args) => Debug($"ETL process {Name} is finished. Duration: {args.DurationSeconds:N3} seconds");
            OnSuccess += (_, args) => Debug($"ETL process {Name} is succeed. Duration: {args.DurationSeconds:N3} seconds");
            OnFailure += (_, args) => Debug($"Error in ETL process {Name}!\n{args.ErrorMessage}");
            OnComplete += (_, args) => Debug($"Process ended at {args.EndTime}");

            State = new EtlState();
        }

        public DataDestination Destination
        {
            get => _destination;
            set
            {
                if (value != null)
                {
                    _destination = value; _destination.ParentEtl = this;
                }
            }
        }

        public DataTransformation Transformation
        {
            get => _transformation;
            set
            {
                if (value != null)
                {
                    _transformation = value; _transformation.ParentEtl = this;
                }
            }
        }

        public DataSource Source
        {
            get => _source;
            set
            {
                if (value != null)
                {
                    _source = value; _source.ParentEtl = this;
                }
            }
        }

        protected virtual void Execute(CancellationToken token = default)
        {
            Debug("Execute ETL process");

            Source.GetData(token).Transform(Transformation).Put(Destination, token);
        }

        public virtual void Run(CancellationToken token = default)
        {
            if (!State.IsActive)
            {
                Debug("Process IsActive = false");
                return;
            }

            try
            {
                State.StartTime = DateTime.Now;

                OnStart?.Invoke(this, State);

                PreExecute(token);
                Execute(token);
                PostExecute(token);

                State.IsSuccessful = true;
                Finish();

                OnSuccess?.Invoke(this, State);
            }
            catch (Exception e)
            {
                State.IsSuccessful = false;
                State.ErrorMessage = e.ToLogString();
                Finish();

                OnFailure?.Invoke(this, State);
            }

            OnFinish?.Invoke(this, State);
            OnComplete?.Invoke(this, State);
        }

        protected virtual void PreExecute(CancellationToken token = default)
        {
            Debug("Start ETL process");
        }

        protected virtual void PostExecute(CancellationToken token = default)
        {
            Debug("End ETL process");
        }

        public override void Dispose()
        {
            Source?.Dispose();
            Transformation?.Dispose();
            Destination?.Dispose();
            base.Dispose();
        }

        protected void Finish()
        {
            State.EndTime = DateTime.Now;
            State.DurationSeconds = (State.EndTime - State.StartTime).TotalMilliseconds / 1000.0;
        }
    }
}