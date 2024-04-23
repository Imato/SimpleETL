using System;
using System.Collections.Generic;

namespace Imato.SimpleETL
{
    public interface IEtlDataFlow
    {
        IEnumerable<EtlColumn> Columns { get; }
        int ColumnsCount { get; }

        EtlColumn AddColumn(string name, Type type);

        EtlColumn AddColumn(EtlColumn column);

        EtlColumn AddColumn<T>(string name);

        bool HasColumn(string name);

        EtlColumn? GetColumn(string name);

        EtlColumn? GetColumn(int id);
    }
}