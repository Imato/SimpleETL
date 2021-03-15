using System;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Configuration;


namespace SimpleETL
{
    public class JsonConfigurationService : IConfigurationService
    {
        private string _fileName;

        public JsonConfigurationService(string fileName = "appsettings.json")
        {            
            _fileName = fileName?.Replace(".", $".{ConfigurationUtils.GetEnvironmentName()}.");
        }
        /// <summary>
        /// Get configuration T from local settings.json file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetConfiguration<T>()
        {
            var json = File.ReadAllText(_fileName);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(_fileName, optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
