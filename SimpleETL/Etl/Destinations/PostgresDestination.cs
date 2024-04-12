using Npgsql;
using System.Data;

namespace Imato.SimpleETL
{
    public class PostgresDestination : DataDestination
    {
        private readonly NpgsqlConnection _connection;
        private readonly string _tableName;
        private readonly IEnumerable<string> _columns;
        private readonly int _batchSize;
        private readonly TimeSpan _timeout;
        private readonly Queue<IEtlRow> _buffer;

        public PostgresDestination(string connectionString,
            string tableName,
            IEnumerable<string> columns,
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
            _buffer = new Queue<IEtlRow>(_batchSize);
        }

        public override void PutData(IEtlRow row, CancellationToken token = default)
        {
            _buffer.Enqueue(row);
            RowAffected++;
            if (_buffer.Count >= _batchSize)
            {
                BulkInsert();
            }
        }

        public override void PutData(IEnumerable<IEtlRow> data, CancellationToken token = default)
        {
            base.PutData(data, token);
            BulkInsert();
            Debug($"Saved {RowAffected} rows total");
        }

        private void BulkInsert()
        {
            if (_buffer.Count == 0)
            {
                return;
            }

            var columns = string.Join(",", _columns);
            Debug($"Write buffer to table {_tableName}: {columns}");
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            using (var writer = _connection.BeginBinaryImport($"copy {_tableName} ({columns}) from STDIN (FORMAT BINARY)"))
            {
                writer.Timeout = _timeout;
                while (_buffer.Count > 0)
                {
                    var row = _buffer.Dequeue();
                    writer.StartRow();
                    foreach (var column in _columns)
                    {
                        var value = row[column];
                        if (value != null)
                        {
                            writer.Write(row[column]);
                        }
                        else
                        {
                            writer.WriteNull();
                        }
                    }
                }

                writer.Complete();
            }

            Debug($"Wrote {_buffer.Count} rows");
        }

        public override void Dispose()
        {
            BulkInsert();

            if (_connection?.State != ConnectionState.Closed)
            {
                _connection?.Close();
            }
            base.Dispose();
        }
    }
}