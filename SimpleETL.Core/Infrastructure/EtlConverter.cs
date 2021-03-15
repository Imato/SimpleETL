using Newtonsoft.Json.Linq;
using System;


namespace SimpleETL
{
    public static class EtlConverter
    {
        public static T TryGetValue<T, V>(V value)
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

        public static object GetTypedValue(this JToken jtocken)
        {
            if (jtocken != null)
            {
                switch (jtocken.Type)
                {
                    case JTokenType.None:
                        return null;
                    case JTokenType.Object:
                        return jtocken.Value<object>();
                    case JTokenType.Array:
                        return jtocken.Value<Array>();                  
                    case JTokenType.Integer:
                        return jtocken.Value<int>();
                    case JTokenType.Float:
                        return jtocken.Value<double>();
                    case JTokenType.String:
                        return jtocken.Value<string>();
                    case JTokenType.Boolean:
                        return jtocken.Value<bool>();                   
                    case JTokenType.Date:
                        return jtocken.Value<DateTime>();
                    case JTokenType.Guid:
                        return jtocken.Value<Guid>();
                    case JTokenType.Uri:
                        return jtocken.Value<Uri>();
                    case JTokenType.TimeSpan:
                        return jtocken.Value<TimeSpan>();
                    default:
                        return null;
                }
            }

            return null;
        }
    }
}
