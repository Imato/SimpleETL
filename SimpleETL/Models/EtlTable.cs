namespace Imato.SimpleETL
{
    public class EtlTable : IDisposable, IEtlTable
    {
        private IEtlRow[] _table;
        private int _current;

        public EtlTable(int bufferSize = 1000)
        {
            BufferSize = bufferSize;
            Clear();
        }

        public IEnumerable<IEtlRow> Rows
        {
            get { return _table.Where(x => x != null); }
        }

        public void Clear()
        {
            _table = new IEtlRow[BufferSize];
            _current = -1;
        }

        public void Dispose()
        {
            _table = null;
        }

        public int RowCount
        {
            get { return _current + 1; }
        }

        public int BufferSize { get; }

        public IEtlRow this[int row] =>
            row <= _current && row >= 0 ? _table[row] : throw new IndexOutOfRangeException(nameof(row));

        public void AddRow(IEtlRow row)
        {
            if (_current >= BufferSize - 1)
            {
                throw new Exception($"Buffer size {BufferSize} exceeded");
            }

            _table[++_current] = row;
        }
    }
}