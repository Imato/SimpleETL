using System;
using System.Linq;
using System.Collections.Generic;

namespace SimpleETL
{
    public class EtlDataFlow : IEtlDataFlow
    {
        private HashSet<EtlColumn> columns;
        private int countColumns = -1;

        public EtlDataFlow()
        {
            columns = new HashSet<EtlColumn>(20);
        }

        public IEnumerable<EtlColumn> Columns => columns;

        public int ColumnsCount => countColumns + 1;

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

        public EtlColumn GetColumn(string name)
        {
            columns.TryGetValue(new EtlColumn(0, name, name.GetType()), out var column);
            return column;
        }

        public EtlColumn GetColumn(int id)
        {
            return columns.Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
