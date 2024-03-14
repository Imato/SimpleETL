using System.Collections.Generic;
using System;
using System.Linq;

namespace SimpleETL
{
    public class ListDestination<T> : DataDestination where T : class
    {
        private List<T> _data;
        private string _sourceColumn;

        public ListDestination(string sourceColumn)
        {
            _data = new List<T>();
            _sourceColumn = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));
            RowAffected = 0;
        }

        public override void PutData(IEnumerable<IEtlRow> data)
        {
            foreach (var row in data)
            {
                PutData(row);
            }
        }

        public override void PutData(IEtlRow row)
        {

            if (row[_sourceColumn] != null
                    && row[_sourceColumn].GetType() == typeof(T))
            {
                _data.Add(row[_sourceColumn] as T);
                RowAffected++;
            }
        }

        public List<T> GetData()
        {
            return _data;
        }
    }

    public class ListDestination : DataDestination
    {
        private List<IEtlRow> _data;

        public ListDestination()
        {
            _data = new List<IEtlRow>();
            RowAffected = 0;
        }

        public override void PutData(IEnumerable<IEtlRow> data)
        {
            foreach (var row in data)
            {
                PutData(row);
            }
        }

        public override void PutData(IEtlRow row)
        {
            _data.Add(row);
            RowAffected++;
        }
        public List<IEtlRow> GetData()
        {
            return _data;
        }
    }
}
