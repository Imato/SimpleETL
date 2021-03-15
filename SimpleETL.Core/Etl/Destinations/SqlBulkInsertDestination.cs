using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SimpleETL
{
    public class SqlBulkInsertDestination : DataDestination
    {
        private SqlConnection _connection;
        private SqlBulkCopy _bulk;
        private EtlTable _buffer;
        private object _lock;

        public SqlBulkInsertDestination(string connectionString, 
            string tableName, 
            int bufferSize = 10000, 
            IList<string> columns = null,
            EtlObject parent = null)
        {
            _buffer = new EtlTable(bufferSize);
            _connection = new SqlConnection(connectionString);
            _bulk = new SqlBulkCopy(_connection)
            {
                BatchSize = bufferSize,
                DestinationTableName = tableName,
                BulkCopyTimeout = 30000
            }; 
            
            if(columns != null)
            {
                foreach (var c in columns)
                {
                    _bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping(c, c));
                }
            }

            if(parent != null)
                ParentEtl = parent;

            _lock = new object();
        }

        public override void Dispose()
        {
            WriteToServer();

            if (_connection.State != System.Data.ConnectionState.Closed)
                _connection.Close();
            _bulk.Close();
            _buffer.Dispose();
            base.Dispose();
        }

        public override void PutData(IEtlRow row)
        {
            Open();
            _buffer.AddRow(row);

            if (_buffer.RowCount == _bulk.BatchSize)
                WriteToServer();

            RowAffected++;
        }

        public override void PutData(IEnumerable<IEtlRow> data)
        {
            Log($"Try to put data in sql destination table {_bulk.DestinationTableName}");

            Open();
            foreach (var row in data)
            {
                PutData(row);
            }
            
            WriteToServer();

            Log($"Saved {RowAffected} rows total");
        }

        private void WriteToServer()
        {
            lock (_lock)
            {
                if (_buffer.RowCount > 0)
                {
                    _bulk.WriteToServer(new EtlReader(_buffer));
                    _buffer.Clear();
                }
            }                            
        }

        private void Open()
        {
            if(_connection.State != System.Data.ConnectionState.Open)
                _connection.Open();
        }

    }
}
