using System.Collections.Generic;
using System;

namespace Imato.SimpleETL
{
    public class DictionaryDestination : DataDestination, IDataSource

    {
        private Dictionary<object, object> _data;
        private string _keyColumn, _valueColumn;

        public DictionaryDestination(string keyColumn, string valueColumn) : base()
        {
            if (string.IsNullOrEmpty(keyColumn))
                throw new ArgumentNullException(nameof(keyColumn));

            if (string.IsNullOrEmpty(valueColumn))
                throw new ArgumentNullException(nameof(valueColumn));

            _data = new Dictionary<object, object>();
            _keyColumn = keyColumn;
            _valueColumn = valueColumn;
        }

        public override void PutData(IEtlRow row)
        {
            if (row[_keyColumn] != null)
            {
                var key = row[_keyColumn];
                var value = row[_valueColumn];
                if (_data.ContainsKey(key))
                    _data[key] = value;
                else
                    _data.Add(key, value);
            }
        }

        public object? GetData(object? key)
        {
            if (key != null && _data.ContainsKey(key))
            {
                return _data[key];
            }

            return null;
        }

        public IEnumerable<IEtlRow> GetData()
        {
            var flow = new EtlDataFlow();

            foreach (var r in _data)
            {
                var row = new EtlRow(flow);
                row[_keyColumn] = r.Key;
                row[_valueColumn] = r.Value;
                yield return row;
            }
        }
    }
}