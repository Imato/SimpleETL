namespace Imato.SimpleETL
{
    public class EtlProcess : EtlObject, IEtlProcess
    {
        private DataSource? _source;
        private DataTransformation? _transformation;
        private DataDestination? _destination;

        public DataDestination? Destination
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

        public DataTransformation? Transformation
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

        public DataSource? Source
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

        protected virtual void Execute()
        {
            Log("Execute ETL process");

            Source?.GetData()?.Transform(Transformation)?.Put(Destination);

            if (Source?.RowAffected != null)
            {
            }

            Source?.Dispose();
            Transformation?.Dispose();
            Destination?.Dispose();
        }

        public void Run()
        {
            var etlStatus = new EtlStatusEventArgs()
            {
                StartTime = DateTime.Now,
                IsSuccessful = false
            };

            try
            {
                OnStart?.Invoke(this, etlStatus);

                PreExecute();
                Execute();
                PostExecute();

                etlStatus.EndTime = DateTime.Now;
                etlStatus.IsSuccessful = true;
                etlStatus.DurationSecconds = (etlStatus.EndTime - etlStatus.StartTime).TotalMilliseconds / 1000.0;

                OnSuccess?.Invoke(this, etlStatus);
            }
            catch (Exception e)
            {
                etlStatus.IsSuccessful = false;
                etlStatus.ErrorMessage = e.ToLogString();

                OnFailure?.Invoke(this, etlStatus);
            }

            OnFinish?.Invoke(this, etlStatus);
            OnComplete?.Invoke(this, etlStatus);
        }

        protected virtual void PreExecute()
        {
            Log("Start ETL process");
        }

        protected virtual void PostExecute()
        {
            Log("End ETL process");

            Source?.Dispose();
            Transformation?.Dispose();
            Destination?.Dispose();
        }

        public EtlEventHandler? OnStart;
        public EtlEventHandler? OnFinish;
        public EtlEventHandler? OnSuccess;
        public EtlEventHandler? OnFailure;
        public EtlEventHandler? OnComplete;
    }
}