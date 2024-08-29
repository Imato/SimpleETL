using System.Data.SqlClient;
using System.Data;

namespace Imato.SimpleETL
{
    public class MsSqlDestination : DataDestination
    {
        private readonly SqlConnection _connection;
        private readonly SqlBulkCopy _bulk;
        private readonly EtlTable _buffer;

        public MsSqlDestination(string connectionString,
            string tableName,
            int bufferSize = 10000,
            IEnumerable<string>? columns = null,
            EtlObject? parent = null)
        {
            _buffer = new EtlTable(bufferSize + 100);
            _connection = new SqlConnection(connectionString);
            _bulk = new SqlBulkCopy(_connection, SqlBulkCopyOptions.KeepIdentity, null)
            {
                BatchSize = bufferSize,
                DestinationTableName = tableName,
                BulkCopyTimeout = 30000
            };

            if (columns != null)
            {
                foreach (var c in columns)
                {
                    _bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping(c, c));
                }
            }

            if (parent != null)
                ParentEtl = parent;
        }

        public override void Dispose()
        {
            WriteToServer();

            if (_connection.State != ConnectionState.Closed)
                _connection.Close();
            _bulk.Close();
            _buffer.Dispose();
            base.Dispose();
        }

        public override void PutData(IEtlRow row, CancellationToken token = default)
        {
            _buffer.AddRow(row);

            if (_bulk.ColumnMappings.Count == 0)
            {
                foreach (var column in row.Flow.Columns)
                {
                    _bulk.ColumnMappings
                        .Add(new SqlBulkCopyColumnMapping(column.Name, column.Name));
                }
            }

            if (_buffer.RowCount >= _bulk.BatchSize)
                WriteToServer();

            RowsAffected++;
        }

        public override void PutData(IEnumerable<IEtlRow> data, CancellationToken token = default)
        {
            Debug($"Try to put data in sql destination table {_bulk.DestinationTableName}");

            foreach (var row in data)
            {
                PutData(row);
            }

            WriteToServer();

            Debug($"Saved {RowsAffected} rows total");
        }

        private void WriteToServer()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            if (_buffer.RowCount > 0)
            {
                _bulk.WriteToServer(new EtlReader(_buffer));
                _buffer.Clear();
            }
        }
    }
}