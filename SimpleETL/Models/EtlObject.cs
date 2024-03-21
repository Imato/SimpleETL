using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Imato.SimpleETL
{
    public class EtlObject : IDisposable
    {
        protected ILogger Logger;

        public string Name { get; set; }

        public EtlObject? ParentEtl { get; set; }

        public EtlObject()
        {
            Logger = EtlContext.Services
                .GetRequiredService<ILoggerProvider>()
                .CreateLogger(GetType()?.FullName ?? nameof(EtlObject));
            Name ??= GetType().Name;
            Debug("Created");
        }

        public virtual void Dispose()
        {
            Debug("Closed");
        }

        protected void Log(object message)
        {
            Logger?.LogInformation(Json.Serialize(message));
        }

        protected void Error(object message)
        {
            Logger?.LogError(Json.Serialize(message));
        }

        protected void Debug(object message)
        {
            Logger?.LogDebug(Json.Serialize(message));
        }

        protected void Warning(object message)
        {
            Logger?.LogWarning(Json.Serialize(message));
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