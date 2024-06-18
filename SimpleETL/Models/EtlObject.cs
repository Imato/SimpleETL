using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Imato.Logger.Extensions;

namespace Imato.SimpleETL
{
    public class EtlObject : IEtlObject
    {
        protected ILogger Logger;

        public string Name { get; set; }

        public EtlObject? ParentEtl { get; set; }

        public EtlObject()
        {
            Logger = EtlContext.Services
                .GetRequiredService<ILoggerProvider>()
                .CreateLogger(GetType()?.FullName ?? nameof(EtlObject));
            Name ??= GetType().FullName!;
            Debug("Created");
        }

        public virtual void Dispose()
        {
            Debug("Closed");
        }

        protected void Debug(object message)
        {
            Logger?.LogDebug(() => $"{Name}: {Json.Serialize(message)}");
        }

        protected void Log(object message)
        {
            Logger?.LogInformation(() => $"{Name}: {Json.Serialize(message)}");
        }

        protected void Warning(object message)
        {
            Logger?.LogWarning(() => $"{Name}: {Json.Serialize(message)}");
        }

        protected void Error(object message, Exception? ex = null)
        {
            if (ex != null)
            {
                Logger?.LogError(ex, () => $"{Name}: {Json.Serialize(message)}");
            }
            Logger?.LogError(() => $"{Name}: {Json.Serialize(message)}");
        }

        public override string ToString()
        {
            if (ParentEtl != null)
                return $"{ParentEtl}/{Name}";
            else
                return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + 230094;
        }
    }
}