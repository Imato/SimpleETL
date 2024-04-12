using System.Data.Common;
using System.Collections;

namespace Imato.SimpleETL
{
    public class EtlReader : DbDataReader
    {
        private readonly IEtlTable source;
        private readonly IEtlDataFlow flow;

        private int currentRow = -1;

        public EtlReader(IEtlTable source)
        {
            if (source == null && source.RowCount > 0)
                throw new ArgumentNullException(nameof(source));

            this.source = source;
            flow = source[0].Flow;
        }

        private object GetColumn(int ordinal)
        {
            return source[currentRow][ordinal];
        }

        private object GetColumn(string name)
        {
            return source[currentRow][name];
        }

        public override object this[int ordinal] => GetColumn(ordinal);

        public override object this[string name] => GetColumn(name);

        public override int Depth
        {
            get { return 0; }
        }

        public override int FieldCount => flow.ColumnsCount;

        public override bool HasRows => currentRow < source.RowCount;

        public override bool IsClosed => currentRow >= source.RowCount;

        public override int RecordsAffected => currentRow;

        public override bool GetBoolean(int ordinal)
        {
            return (bool)GetColumn(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return (byte)GetColumn(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            return (char)GetColumn(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            return GetFieldType(ordinal).Name;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return (DateTime)GetColumn(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return (decimal)GetColumn(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return (double)GetColumn(ordinal);
        }

        public override IEnumerator GetEnumerator()
        {
            return source.Rows.GetEnumerator();
        }

        public override Type GetFieldType(int ordinal)
        {
            return GetColumn(ordinal).GetType();
        }

        public override float GetFloat(int ordinal)
        {
            return (float)GetColumn(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return (Guid)GetColumn(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return (short)GetColumn(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return (int)GetColumn(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return (long)GetColumn(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return flow.GetColumn(ordinal)?.Name;
        }

        public override int GetOrdinal(string name)
        {
            var column = flow.GetColumn(name);
            if (column == null)
            {
                throw new IndexOutOfRangeException(nameof(name));
            }
            return column.Id;
        }

        public override string GetString(int ordinal)
        {
            return (string)GetColumn(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            return GetColumn(ordinal);
        }

        public override int GetValues(object[] values)
        {
            values = source[currentRow].GetValues().ToArray();
            return source[currentRow].ColumnsCount;
        }

        public override bool IsDBNull(int ordinal)
        {
            return GetColumn(ordinal) == null;
        }

        public override bool NextResult()
        {
            return HasRows;
        }

        public override bool Read()
        {
            currentRow++;
            return HasRows;
        }
    }
}