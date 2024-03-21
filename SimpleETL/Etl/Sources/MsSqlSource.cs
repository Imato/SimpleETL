using System.Data.SqlClient;
using System.Text;

namespace Imato.SimpleETL
{
    public class MsSqlSource : DataSource
    {
        private readonly SqlConnection _connection;
        private readonly string _sqlQuery;
        private readonly IDictionary<string, object> _parameters;
        private readonly int _timeOut;

        public MsSqlSource(string connectionString, string sqlQuery, IDictionary<string, object> parameters = null, int timeOut = 30)
        {
            _connection = new SqlConnection(connectionString);
            _sqlQuery = sqlQuery;
            _parameters = parameters;
            _timeOut = timeOut;
        }

        public override void Dispose()
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
                _connection.Close();

            Log($"Finish getting data from SQL source. {RowAffected} rows");
            base.Dispose();
        }

        public override IEnumerable<IEtlRow> GetData()
        {
            _connection.Open();

            using (var command = new SqlCommand(_sqlQuery, _connection))
            {
                command.CommandTimeout = _timeOut;

                // Add parameters

                if (_parameters != null)
                {
                    foreach (var p in _parameters)
                    {
                        var parameter = new SqlParameter
                        {
                            ParameterName = $"@{p.Key}",
                            Value = p.Value
                        };

                        command.Parameters.Add(parameter);
                    }
                }

                PrintCommand(command);

                // Get data

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = CreateRow();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = !reader.IsDBNull(i) ? reader.GetValue(i) : null;
                        }

                        RowAffected++;
                        yield return row;
                    }
                }
            }
        }

        private void PrintCommand(SqlCommand command)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Get data from SQL source by query:");
            for (int i = 0; i < command.Parameters.Count; i++)
            {
                var p = command.Parameters[i];
                sb.AppendLine($"declare {p.ParameterName} varchar(255) = '{p.Value}'");
            }
            sb.AppendLine($"{command.CommandText}");
            Log(sb.ToString());
        }
    }
}