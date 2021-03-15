using System;
using System.Text;

namespace SimpleETL
{
    public class EtlObject : IDisposable
    {
        private ILogger _logger;

        private string _name;

        public string Name
        {
            get { return _name ?? GetType().Name; }
            set { _name = value; } 
        }

        public EtlObject ParentEtl { get; set; }

        public EtlObject()
        {
            _logger = EtlContext.GetContext().Logger;
            Debug("Created");
        }

        public EtlObject(string name) : this()
        {
            Name = name;            
        }

        protected T GetConfiguration<T>()
        {
            return EtlContext.GetContext().GetConfiguration<T>();
        }

        protected void SetConfiguration<T>(T configuration)
        {
            EtlContext.GetContext().SetConfiguration(configuration);
        }

        public virtual void Dispose()
        {
            Debug("Closed");
        }

        protected void Log(object message)
        {
            _logger?.Information(ToString(), message);
        }

        protected void Error(object message)
        {
            _logger?.Error(ToString(), message);
        }

        protected void Debug(object message)
        {
            _logger?.Debug(ToString(), message);
        }

        public override string ToString()
        {
            if (ParentEtl != null)
                return $"{ParentEtl}/{Name}";
            else
                return Name;
        }

        
    }
}
