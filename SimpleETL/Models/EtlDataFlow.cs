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

        public EtlColumn AddColumn(EtlColumn column)
        {
            return AddColumn(column.Name, column.Type);
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
                    column.Type = type;
                }

                return column;
            }

            countColumns++;
            column = new EtlColumn
            {
                Id = countColumns,
                Name = name,
                Type = type
            };
            columns.Add(column);
            return column;
        }

        public bool HasColumn(string name)
        {
            return GetColumn(name) != null;
        }

        public EtlColumn? GetColumn(string name)
        {
            columns.TryGetValue(new EtlColumn { Name = name }, out var column);
            return column;
        }

        public EtlColumn? GetColumn(int id)
        {
            return columns.Where(x => x.Id == id).FirstOrDefault();
        }
    }
}