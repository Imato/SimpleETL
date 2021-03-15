using System;
using System.Threading.Tasks;
using SimpleETL;
using DashboardETL.Infrastructure;
using DashboardETL.Models;
using DashboardETL.ETL;
using System.Reflection;

namespace DashboardETL
{
    public class EtlRunner
    {
        private static ILogger _logger;
        private static Configuration _config;
        private EtlRunner() { }

        public static EtlRunner Build(string[] args = null)
        {
            var runner = new EtlRunner();

            // Get settings
            var context = EtlContext.GetContext();
            context.ConfigurationService = new JsonConfigurationService();
            _config = context.ConfigurationService.GetConfiguration<Configuration>();

            if (SimpleETL.ConfigurationUtils.IsDevelopment)
            {
                _logger = new ConsoleLogger(context.ConfigurationService);
            }
            else
            {

                var app = Assembly.GetAssembly(typeof(Program)).GetName();
                _logger = new SqlServerLogger(context.ConfigurationService,
                    _config.GetConnectionString(),
                    "adm.Log",
                    $"{app.Name} {app.Version}");
            }

            context.Logger = _logger;

            _logger.Information("DashboardETL", $"Start process with parameters: {(args == null ? "" : string.Join(',', args))}");
            _logger.Information("DashboardETL", $"Use SQL store: {Infrastructure.ConfigurationUtils.GetConnectionString()} ");
            _logger.Information("DashboardETL", $"Environment: {SimpleETL.ConfigurationUtils.GetEnvironmentName()}");

            return runner;

        }

        public void Run()
        {
            try
            {
                Task.WaitAll(new MainPackage().RunAsync());
            }
            catch (Exception e)
            {
                _logger.Error("DashboardETL", e);
            }

            _logger.Information("DashboardETL", "End program");
            _logger.Dispose();
        }
    }
}
