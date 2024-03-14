using SimpleETL.Models;
using System;

namespace SimpleETL
{
    public class ConsoleLogger : BaseLoger
    {

        public ConsoleLogger(IConfigurationService configuration):base(configuration)
        {
        }        

        public override void WriteLog(object source, string message, LogLevel level)
        {
            Console.WriteLine($"{DateTime.Now:dd-MM-yyyy HH:mm:ss.mmm} {level} {source}: {message}");
        }

        public override void Dispose()
        {

        }
    }
}
