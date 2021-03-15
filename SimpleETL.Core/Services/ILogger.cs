using System;

namespace SimpleETL
{
    public interface ILogger : IDisposable
    {
        void Information(object source, object message);
        void Debug(object source, object message);
        void Error(object source, object message);
        void Error(object source, Exception exception);
    }
}
