using SimpleETL.Models;
using System;
using System.Data.SqlClient;

namespace SimpleETL
{
    public class SqlServerLogger : BaseLoger
    {
        private readonly string _connectionString;
        private readonly string _sql;
        private static string _null = "NULL";

        public SqlServerLogger(IConfigurationService configuration, string connectionString, string tableName, string appName = null) : base(configuration)
        {
            _connectionString = connectionString;
            appName = appName ?? ConfigurationUtils.GetAppName();
            _sql = @"
                insert into " + tableName + @"
                (TimeStamp, Application, LogEvent, Level, Message, Exception, Server)
                values 
                ('{0}', '" + appName + "', '{1}', '{2}', '{3}', '{4}', '" + Environment.MachineName + "');";
        }

        public override void WriteLog(object source, string message, LogLevel level)
        {
            var com = string.Format(_sql,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                FormatString(source),
                level,  
                level != LogLevel.Error ? FormatString(message) : "",
                level == LogLevel.Error ? FormatString(message) : "");

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(com, connection))
                    command.ExecuteNonQuery();
            }
                
        }

        private string FormatString(object data)
        {
            return (data ?? _null).ToString().Replace("'", "''");
        }

        public override void Dispose()
        {
            
        }
    }
}
