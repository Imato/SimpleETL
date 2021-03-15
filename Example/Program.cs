using SimpleETL;

namespace DashboardETL
{
    static class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">Command line arguments:
        /// -e: Environment: Development, Test, Production</param>
        /// <returns></returns>
        static void Main(string[] args)
        {
            var argParser = new ArgumentsParser(args);
            ConfigurationUtils.SetEnvironment(argParser.Get("-e")?.ToString());

            EtlRunner.Build(args).Run();
        }
    }
}
