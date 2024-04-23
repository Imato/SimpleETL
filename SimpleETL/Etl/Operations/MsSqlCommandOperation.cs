using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Imato.SimpleETL
{
    public class MsSqlCommandOperation : BaseOperation
    {
        protected readonly string ConnectionString;
        protected readonly string SqlCommand;
        protected readonly int Timeout;

        public MsSqlCommandOperation(string connectionString,
            string sqlCommand,
            EtlObject? parentEtl = null,
            int timeout = 120)
        {
            ParentEtl = parentEtl;
            ConnectionString = connectionString;
            SqlCommand = sqlCommand;
            Timeout = timeout;
        }

        public virtual void Run()
        {
            Run(null);
        }

        public virtual void Run(object? parameters)
        {
            Run(ObjectMapper.GetFields(parameters));
        }

        public virtual void Run(IDictionary<string, object>? parameters)
        {
            if (!string.IsNullOrEmpty(ConnectionString) && !string.IsNullOrEmpty(SqlCommand))
            {
                Debug($"Run sql operation {SqlCommand}");

                try
                {
                    using (var c = new SqlConnection(ConnectionString))
                    {
                        c.Open();
                        IDbCommand cmd = c.CreateCommand();
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
                                    cmd.Parameters.Add(new SqlParameter
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