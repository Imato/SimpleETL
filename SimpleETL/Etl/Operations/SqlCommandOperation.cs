using Dapper;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SimpleETL
{
    public class SqlCommandOperation : BaseOperation
    {
        protected readonly string _connectionString;
        protected readonly string _sqlCommand;
        private readonly int _timeout;

        public SqlCommandOperation(string connectionString, string sqlCommand, EtlObject parentEtl = null, int timeout = 120)
        {
            Debug("Create sql operation");

            ParentEtl = parentEtl;
            _connectionString = connectionString;
            _sqlCommand = sqlCommand;
            _timeout = timeout;
        }

        public override void Run()
        {
            Run(null);
        }

        public void Run(object parameters)
        {
            if (!string.IsNullOrEmpty(_connectionString) && !string.IsNullOrEmpty(_sqlCommand))
            {
                Debug($"Run sql operation {_sqlCommand}");

                try
                {
                    using (var c = new SqlConnection(_connectionString))
                    {
                        c.Open();
                        c.Execute(_sqlCommand, parameters, null, _timeout);
                    }
                }
                catch
                {
                    Error($"Error in sql command: {_sqlCommand}");
                    throw;
                }
            }
        }

        public override async Task RunAsync()
        {
            await RunAsync(null);
        }

        public async Task RunAsync(object parameters)
        {
            if (!string.IsNullOrEmpty(_connectionString) && !string.IsNullOrEmpty(_sqlCommand))
            {
                Debug($"Run sql operation {parameters ?? ""}");

                using (var c = new SqlConnection(_connectionString))
                {
                    c.Open();
                    await c.ExecuteAsync(_sqlCommand, parameters, null, _timeout);
                }
            }
        }

        public override void Dispose()
        {
            Debug("Close sql operation");
        }
    }
}