using System;
using System.Threading.Tasks;

namespace SimpleETL
{
    public class EtlProcess : EtlObject, IEtlProcess
    {
        private DataSource _source;
        private DataTransformation _transformation;
        private DataDestination _destination;

        public EtlProcess() { }

        public EtlProcess(string name) : base(name) { }

        public EtlProcess(string name, DataSource source, DataTransformation transformation, DataDestination destination) : base(name)
        {
            Source = source;
            Transformation = transformation;
            Destination = destination;
        }

        public DataDestination Destination
        {
            get => _destination;
            set { _destination = value; _destination.ParentEtl = this; }
        }

        public DataTransformation Transformation
        {
            get => _transformation;
            set { _transformation = value; _transformation.ParentEtl = this; }
        }
        public DataSource Source
        {
            get => _source;
            set { _source = value; _source.ParentEtl = this; }
        }        

        protected virtual void Execute()
        {
            Source?.GetData()?.Transform(Transformation)?.Put(Destination);

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


        public async Task RunAsync()
        {
            await Task.Run(Run);
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

        public EtlEventHandler OnStart;
        public EtlEventHandler OnFinish;
        public EtlEventHandler OnSuccess;
        public EtlEventHandler OnFailure;
        public EtlEventHandler OnComplete;

    }   

}
