using Imato.SimpleETL.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;

namespace Imato.SimpleETL
{
    public static class ConfigurationUtils
    {
        private static string DEVELOPMENT = "Development";
        private static string PRODUCTION = "Production";
        private static string _environment = null!;

        public static string GetAppName()
        {
            var app = Assembly.GetEntryAssembly()?.GetName();
            return app != null ? $"{app?.Name} {app?.Version}" : "Unknown";
        }

        public static bool IsDevelopment => GetEnvironmentName() == DEVELOPMENT;
        public static bool IsProduction => GetEnvironmentName() == PRODUCTION;

        public static void SetEnvironment(string? environment)
        {
            _environment = environment ?? _environment;
        }

        public static string GetEnvironmentName()
        {
            if (!string.IsNullOrEmpty(_environment))
                return _environment;

            _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT")
                ?? DEVELOPMENT;

            return _environment;
        }

        public static string ConnectionString(this IConfiguration configuration, string? name = null)
        {
            var connectionString = configuration.GetSection("ConnectionStrings").GetChildren()
                .Where(x => name == null || x.Key == name)
                .FirstOrDefault()
                ?.Value ?? throw new Exception("Cannot find connection string in configuration");
            return AppEnvironment.GetVariables(connectionString);
        }
    }
}