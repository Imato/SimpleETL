using System;
using System.Collections.Generic;

namespace SimpleETL
{
    public class EtlRow : IEtlRow
    {
        private readonly IEtlDataFlow flow;
        private object?[] data;

        private EtlRow(object[] data, IEtlDataFlow flow)
        {
            this.data = CreateData(flow);
            Array.Copy(data, this.data, data.Length);
            this.flow = flow;
        }

        public EtlRow() : this(new EtlDataFlow())
        {
        }

        public EtlRow(IEtlDataFlow flow)
        {
            this.flow = flow;
            data = CreateData(flow);
        }

        private object?[] CreateData(IEtlDataFlow flow)
        {
            return new object?[flow.ColumnsCount <= 20 ? 20 : flow.ColumnsCount];
        }

        public IEtlRow Create()
        {
            return new EtlRow(flow);
        }

        private void Set(string name, object? value)
        {
            var column = flow.AddColumn(name, value?.GetType() ?? typeof(object));
            if (value == null || column.Type == value.GetType())
            {
                if (data.Length == column.Id)
                {
                    var na = new object[data.Length * 2];
                    Array.Copy(data, na, data.Length);
                    data = na;
                }

                data[column.Id] = value;
            }
            else
            {
                throw new TypeAccessException($"Wrong type {value.GetType().Name} of {nameof(value)}");
            }
        }

        private object? Get(string name)
        {
            var column = flow.GetColumn(name);
            return column != null ? data[column.Id] : null;
        }

        public IEtlDataFlow Flow => flow;

        public int ColumnsCount => flow.ColumnsCount;

        public object? this[string name]
        {
            get
            {
                return Get(name);
            }

            set
            {
                flow.AddColumn(name, value?.GetType() ?? typeof(object));
                Set(name, value);
            }
        }

        public object? this[int id]
        {
            get
            {
                var column = flow.GetColumn(id);
                if (column != null)
                {
                    return data[column.Id];
                }

                return null;
            }
            set
            {
                var column = flow.GetColumn(id);
                if (column != null)
                {
                    Set(column.Name, value);
                }
                else
                {
                    throw new IndexOutOfRangeException($"Wrong id {id}");
                }
            }
        }

        public string ColumnName(int id)
        {
            return flow.GetColumn(id)?.Name ?? throw new IndexOutOfRangeException($"Wrong column id {id}");
        }

        public int ColumnId(string name)
        {
            return flow.GetColumn(name)?.Id ?? throw new IndexOutOfRangeException($"Wrong column name {name}"); ;
        }

        public IEnumerable<object?> GetValues()
        {
            return data;
        }

        public override string ToString()
        {
            return this.Format();
        }

        public bool HasColumn(string name)
        {
            return flow.HasColumn(name);
        }

        public void Clear()
        {
            data = CreateData(flow);
        }

        public EtlRow Copy()
        {
            return new EtlRow(data, flow);
        }
    }
}