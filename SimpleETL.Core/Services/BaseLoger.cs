using SimpleETL.Models;
using System;
using System.Text;

namespace SimpleETL
{
    public abstract class BaseLoger : ILogger, IDisposable
    {
        protected readonly EtlConfiguration _configuration;

        public BaseLoger(IConfigurationService configuration)
        {
            _configuration = configuration.GetConfiguration<EtlConfiguration>();
        }

        public void Debug(object source, object message)
        {
            if(_configuration.GetLogLevel() >= LogLevel.Debug)
                WriteLog(source, message.ToString(), LogLevel.Debug);
        }        

        public void Error(object source, object message)
        {
            if (_configuration.GetLogLevel() >= LogLevel.Error)
                WriteLog(source, message.ToString(), LogLevel.Error);
        }

        public void Information(object source, object message)
        {
            if (_configuration.GetLogLevel() >= LogLevel.Information)
                WriteLog(source, message.ToString(), LogLevel.Information);
        }

        public abstract void WriteLog(object source, string message, LogLevel level);
        public abstract void Dispose();

        public void Error(object source, Exception exception)
        {            
            Error(source, exception.ToLogString());
        }
    }
}
