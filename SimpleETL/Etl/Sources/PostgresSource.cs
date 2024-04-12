using Npgsql;
using System.Text;
using System.Data;

namespace Imato.SimpleETL
{
    public class PostgresSource : DataSource
    {
        private readonly NpgsqlConnection _connection;
        private readonly string _sqlQuery;
        private readonly IDictionary<string, object> _parameters;
        private readonly int _timeOut;

        public PostgresSource(string connectionString, string sqlQuery, IDictionary<string, object> parameters = null, int timeOut = 30)
        {
            _connection = new NpgsqlConnection(connectionString);
            _sqlQuery = sqlQuery;
            _parameters = parameters;
            _timeOut = timeOut;
        }

        public override void Dispose()
        {
            if (_connection.State != ConnectionState.Closed)
                _connection.Close();

            Debug($"Finish getting data from source. {RowAffected} rows");
            base.Dispose();
        }

        public override IEnumerable<IEtlRow> GetData(CancellationToken token = default)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using (var command = new NpgsqlCommand(_sqlQuery, _connection))
            {
                command.CommandTimeout = _timeOut;

                // Add parameters

                if (_parameters != null)
                {
                    foreach (var p in _parameters)
                    {
                        var parameter = new NpgsqlParameter
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
                    while (reader.Read() && !token.IsCancellationRequested)
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

        private void PrintCommand(NpgsqlCommand command)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Get data from SQL source by query:");
            for (int i = 0; i < command.Parameters.Count; i++)
            {
                var p = command.Parameters[i];
                sb.AppendLine($"declare {p.ParameterName} varchar(255) = '{p.Value}'");
            }
            sb.AppendLine($"{command.CommandText}");
            Debug(sb.ToString());
        }
    }
}