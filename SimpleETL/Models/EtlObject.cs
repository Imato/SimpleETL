using Imato.Logger.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Imato.SimpleETL
{
    public class EtlObject : IEtlObject
    {
        protected ILogger Logger;

        public string Name { get; set; }

        public EtlObject? ParentEtl { get; set; }

        public EtlObject()
        {
            var name = GetType()?.FullName ?? nameof(EtlObject);
            Logger = EtlContext.Services
                .GetRequiredService<ILoggerProvider>()
                .CreateLogger(name);
            Name ??= name;
            Debug($"Created {name}");
        }

        public virtual void Dispose()
        {
            Debug($"Closed {Name}");
        }

        protected void Debug(object message, Exception? ex = null)
        {
            if (ex != null)
            {
                Logger?.LogDebug(ex, () => $"{this}: {Json.Serialize(message)}");
            }
            else
            {
                Logger?.LogDebug(() => $"{this}: {Json.Serialize(message)}");
            }
        }

        protected void Log(object message, Exception? ex = null)
        {
            if (ex != null)
            {
                Logger?.LogInformation(ex, () => $"{this}: {Json.Serialize(message)}");
            }
            else
            {
                Logger?.LogInformation(() => $"{this}: {Json.Serialize(message)}");
            }
        }

        protected void Warning(object message, Exception? ex = null)
        {
            if (ex != null)
            {
                Logger?.LogWarning(ex, () => $"{this}: {Json.Serialize(message)}");
            }
            else
            {
                Logger?.LogWarning(() => $"{this}: {Json.Serialize(message)}");
            }
        }

        protected void Error(object message, Exception? ex = null)
        {
            if (ex != null)
            {
                Logger?.LogError(ex, () => $"{this}: {Json.Serialize(message)}");
            }
            else
            {
                Logger?.LogError(() => $"{this}: {Json.Serialize(message)}");
            }
        }

        public override string ToString()
        {
            if (ParentEtl != null)
                return $"{ParentEtl}/{Name ?? ""}";
            else
                return Name ?? "";
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + 230094;
        }
    }
}