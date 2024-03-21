using Newtonsoft.Json.Linq;
using System;

namespace Imato.SimpleETL
{
    public static class EtlConverter
    {
        public static T? TryGetValue<T, V>(V value)
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        public static object? GetTypedValue(this JToken jTocken)
        {
            if (jTocken != null)
            {
                switch (jTocken.Type)
                {
                    case JTokenType.None:
                        return null;

                    case JTokenType.Object:
                        return jTocken.Value<object>();

                    case JTokenType.Array:
                        return jTocken.Value<Array>();

                    case JTokenType.Integer:
                        return jTocken.Value<int>();

                    case JTokenType.Float:
                        return jTocken.Value<double>();

                    case JTokenType.String:
                        return jTocken.Value<string>();

                    case JTokenType.Boolean:
                        return jTocken.Value<bool>();

                    case JTokenType.Date:
                        return jTocken.Value<DateTime>();

                    case JTokenType.Guid:
                        return jTocken.Value<Guid>();

                    case JTokenType.Uri:
                        return jTocken.Value<Uri>();

                    case JTokenType.TimeSpan:
                        return jTocken.Value<TimeSpan>();

                    default:
                        return null;
                }
            }

            return null;
        }
    }
}