using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SimpleETL
{
    public static class EtlRowExtension
    {
        public static string Format(this IEtlRow row, string delimiter = ";")
        {
            var sb = new StringBuilder();

            for (int i = 0; i < row.ColumnsCount; i++)
            {
                sb.Append($"{row.ColumnName(i)}:{row[i] ?? "NULL"}");
                if (i != row.ColumnsCount - 1)
                    sb.Append(delimiter);
            }

            return sb.ToString();
        }

        public static string GetDictionaryValue(this Dictionary<string, string> dic, string key)
        {
            if (dic.ContainsKey(key))
                return dic[key];

            return null;
        }

        public static Type GetValueType(this object value)
        {
            if (value == null)
                return typeof(object);

            return value.GetType();
        }

        public static string GetStringValue(this object value, string defaultValue)
        {
            if (value == null)
                return defaultValue;

            var v = value.ToString().Trim();
            if (string.IsNullOrEmpty(v)
                || v == "NULL"
                || v == "N/A")
                return defaultValue;

            return v;
        }

        public static IEtlRow AddColumn(this IEtlRow row, string name, object value)
        {
            row[name] = value;
            return row;
        }

        public static IEnumerable<IEtlRow> AddColumn(this IEnumerable<IEtlRow> data, string name, object value)
        {
            foreach (var row in data)
            {
                yield return row.AddColumn(name, value);
            }
        }

        public static IEnumerable<IEtlRow> Transform(this IEnumerable<IEtlRow> data, IDataTransformation? transformation)
        {
            if (data != null && transformation != null)
                return transformation.TransformData(data);

            return data;
        }

        public static IEnumerable<IEtlRow> Put(this IEnumerable<IEtlRow> data, IDataDestination? destination)
        {
            if (data != null && destination != null)
                destination.PutData(data);
            return data;
        }

        public static IEnumerable<T> UnionIf<T>(this IEnumerable<T> data1, IEnumerable<T> data2, bool condition) where T : IEtlRow
        {
            if (condition)
                return data1.Union(data2);

            return data1;
        }

        public static IEnumerable<IEtlRow> Map(this IEnumerable<IEtlRow> data, Action<IEtlRow> action)
        {
            foreach (var r in data)
            {
                action(r);
            }
            return data;
        }
    }
}