using Npgsql;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Imato.SimpleETL
{
    public class PostgresCommandOperation : MsSqlCommandOperation
    {
        public PostgresCommandOperation(string connectionString,
            string sqlCommand,
            EtlObject? parentEtl = null,
            int timeout = 120) : base(connectionString, sqlCommand, parentEtl, timeout)
        {
        }

        public override void Run(IDictionary<string, object>? parameters)
        {
            if (!string.IsNullOrEmpty(ConnectionString) && !string.IsNullOrEmpty(SqlCommand))
            {
                Debug($"Run postgres operation {SqlCommand}");

                try
                {
                    using (IDbConnection c = new NpgsqlConnection(ConnectionString))
                    {
                        c.Open();
                        var cmd = c.CreateCommand();
                        cmd.CommandText = SqlCommand;
                        if (parameters?.Count > 0)
                        {
                            foreach (var p in parameters)
                            {
                                var key = p.Key.Replace("@", "");
                                var re = new Regex($"(@{key})\\W*", RegexOptions.IgnoreCase);
                                var m = re.Match(SqlCommand);
                                if (m.Success && m.Groups.Count == 2)
                                {
                                    var parameterName = m.Groups[1].Value;
                                    cmd.Parameters.Add(new NpgsqlParameter
                                    {
                                        ParameterName = parameterName,
                                        Value = p.Value != null ? p.Value : DBNull.Value
                                    });
                                }
                            }
                        }
                        cmd.CommandTimeout = Timeout;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Error($"Error in sql command: {SqlCommand}");
                    throw;
                }
            }
        }
    }
}