using System;
using System.Reflection;

namespace SimpleETL
{
    public static class ConfigurationUtils
    {
        private static string DEVELOPMENT = "Development";
        private static string PRODUCTION = "Production";
        private static string _environment = null!;

        public static string GetAppName()
        {
            var app = Assembly.GetEntryAssembly().GetName();
            return $"{app.Name} {app.Version}";
        }

        public static bool IsDevelopment => GetEnvironmentName() == DEVELOPMENT;
        public static bool IsProduction => GetEnvironmentName() == PRODUCTION;

        public static void SetEnvironment(string env)
        {
            _environment = env ?? _environment;
        }

        public static string GetEnvironmentName()
        {
            if (!string.IsNullOrEmpty(_environment))
                return _environment;

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

#if TEST
            environment = environment ?? "Test";
#endif
#if RELEASE
            environment = environment ?? "Production";
#endif

            _environment = environment ?? DEVELOPMENT;
            return _environment;
        }
    }
}