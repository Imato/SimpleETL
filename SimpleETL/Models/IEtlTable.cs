using System.Collections.Generic;

namespace Imato.SimpleETL
{
    public interface IEtlTable
    {
        IEtlRow this[int row] { get; }
        int BufferSize { get; }
        int RowCount { get; }
        IEnumerable<IEtlRow> Rows { get; }
        void AddRow(IEtlRow row);
        void Clear();
    }
}