using SimpleETL;
using System.Linq;
using DashboardETL.Models;

namespace DashboardETL.ETL
{
    public class StringTrimOperation : EtlObject, IDataRowOperation
    {
        public string GetStringValue(object value)
        {
            if (value == null)
                return Configuration.DefaultStringValue;

            return value.GetStringValue(Configuration.DefaultStringValue);
        }

        public IEtlRow Process(IEtlRow row)
        {
            foreach (var column in row.Flow.Columns
                .Where(x => x.Type == typeof(string) || x.Type == typeof(object))
                .Select(x => x.Name))
            {
                row[column] = GetStringValue(row[column]);
            }

            return row;
        }
    }
}
