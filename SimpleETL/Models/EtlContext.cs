using Imato.SimpleETL.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Imato.SimpleETL
{
    public static class EtlContext
    {
        public static IServiceProvider Services = null!;

        private static void Check()
        {
            if (Services == null)
            {
                throw new ArgumentNullException("EtlContext is not initialized. Add all Services before startup");
            }
        }

        public static T GetService<T>()
        {
            Check();
            return Services.GetRequiredService<T>();
        }

        public static ILogger Logger => GetLogger();

        public static ILogger GetLogger(string? name = null)
        {
            return GetService<ILoggerProvider>()
                .CreateLogger(name ?? nameof(EtlContext));
        }

        public static IConfiguration Configuration => GetService<IConfiguration>();

        public static string ConnectionString(string? name = null)
        {
            Check();
            return Configuration.ConnectionString(name);
        }
    }
}