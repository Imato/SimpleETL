using System;
using System.Reflection;

namespace SimpleETL
{
    public static  class ConfigurationUtils
    {
        private static string DEVELOPMENT = "Development";
        private static string _environment = null;

        public static string GetAppName()
        {
            var app = Assembly.GetEntryAssembly().GetName();
            return $"{app.Name} {app.Version}";
        }

        public static bool IsDevelopment => GetEnvironmentName() == DEVELOPMENT;

        public static void SetEnvironment (string env)
        {
            _environment = env ?? _environment;
        }

        public static string GetEnvironmentName()
        {
            if (!string.IsNullOrEmpty(_environment))
                return _environment;

            _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";

            if (!string.IsNullOrEmpty(_environment))
                return _environment;

#if DEBUG
            _environment = DEVELOPMENT;
#endif
            

#if TEST
            _environment = "Test";
#endif
#if RELEASE
            _environment = "Production";
#endif

            return _environment;
        }
    }
}
