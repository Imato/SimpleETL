using System;
using System.Linq;
using System.Collections.Generic;

namespace Imato.SimpleETL
{
    public class EtlDataFlow : IEtlDataFlow
    {
        private HashSet<EtlColumn> columns;
        private int countColumns = -1;

        public EtlDataFlow() : this(20)
        {
        }

        public EtlDataFlow(int columnsCount)
        {
            columns = new HashSet<EtlColumn>(columnsCount);
        }

        public IEnumerable<EtlColumn> Columns => columns;

        public int ColumnsCount => countColumns + 1;

        public EtlColumn AddColumn<T>(string name)
        {
            var type = typeof(T);
            return AddColumn(name, type);
        }

        public EtlColumn AddColumn(string name, Type type)
        {
            var column = GetColumn(name);
            if (column != null)
            {
                // Clarify object type
                if (type != null
                    && type != typeof(object)
                    && (column.Type == typeof(object) || column.Type == typeof(string)))
                {
                    column.Set(type);
                }

                return column;
            }

            countColumns++;
            column = new EtlColumn(countColumns, name, type);
            columns.Add(column);
            return column;
        }

        public bool HasColumn(string name)
        {
            return GetColumn(name) != null;
        }

        public EtlColumn? GetColumn(string name)
        {
            var column = columns.Where(x => x.Name == name).FirstOrDefault();
            return column;
        }

        public EtlColumn? GetColumn(int id)
        {
            return columns.Where(x => x.Id == id).FirstOrDefault();
        }
    }
}