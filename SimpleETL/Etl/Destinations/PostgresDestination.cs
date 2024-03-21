using Npgsql;
using System.Data;

namespace Imato.SimpleETL.Etl.Destinations
{
    internal class PostgresDestination : DataDestination
    {
        private readonly NpgsqlConnection _connection;
        private readonly string _tableName;
        private readonly IList<string> _columns;
        private readonly int _batchSize;
        private readonly TimeSpan _timeout;
        private readonly List<IEtlRow> _buffer;

        public PostgresDestination(string connectionString,
            string tableName,
            IList<string> columns,
            int batchSize = 10000,
            TimeSpan? timeout = null,
            EtlObject? parent = null)
        {
            _batchSize = batchSize;
            _connection = new NpgsqlConnection(connectionString);
            _tableName = tableName;
            _columns = columns;
            _batchSize = batchSize;
            timeout ??= TimeSpan.FromSeconds(30);
            _timeout = timeout.Value;
            ParentEtl = parent;
            _buffer = new List<IEtlRow>(_batchSize);
        }

        public override void PutData(IEtlRow row)
        {
            _buffer.Add(row);
            if (_buffer.Count == _batchSize)
            {
                BulkInsert();
            }
        }

        private void BulkInsert()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            using (var writer = _connection.BeginBinaryImport($"copy {_tableName} ({string.Join(",", _columns)}) from STDIN (FORMAT BINARY)"))
            {
                writer.Timeout = _timeout;
                foreach (var row in _buffer)
                {
                    writer.StartRow();
                    foreach (var column in _columns)
                    {
                        if (row.HasColumn(column))
                            writer.Write(row[column]);
                    }
                }

                writer.Complete();
            }

            _buffer.Clear();
        }

        public override void Dispose()
        {
            if (_buffer.Count > 0)
                BulkInsert();

            if (_connection?.State != ConnectionState.Closed)
            {
                _connection?.Close();
            }
            base.Dispose();
        }
    }
}