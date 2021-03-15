using DashboardETL.Models;
using System;
using System.Data.SqlClient;
using Dapper;
using SimpleETL;

namespace DashboardETL.Infrastructure
{
    public static class ConfigurationUtils
    {
        public static string GetConnectionString(this Configuration configuration)
        {
            if(configuration.ConnectionStrings != null
                && configuration.ConnectionStrings.ContainsKey(Configuration.DashboardDb))
            {
                return configuration.ConnectionStrings[Configuration.DashboardDb];
            }

            throw new ApplicationException($"Configuration does not contain connection string '{Configuration.DashboardDb}'");
        }

        public static string GetConnectionString()
        {
            return EtlContext
                .GetContext()
                .GetConfiguration<Configuration>()
                .GetConnectionString();
        }

        public static bool IsPrimaryServer(this Configuration configuration)
        {
            var r = false;

            try
            {
                var c = new SqlConnection(configuration.GetConnectionString());
                string sql = @"SELECT TOP 1 1 AS Result
                            FROM master.sys.database_mirroring AS m
                            WHERE m.database_id = DB_ID('" + Configuration.DashboardDb + "')"
                            + " AND (m.mirroring_role_desc = 'PRINCIPAL' OR m.mirroring_role_desc IS NULL)";

                r = c.QueryFirst<bool>(sql);
                return r;
            }

            catch(Exception e)
            {                
                Console.WriteLine(e);                
            }

            Console.WriteLine("This is not primary sql server. Exit.");
            return r;
        }
    }
}
