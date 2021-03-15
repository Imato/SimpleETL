using Newtonsoft.Json.Linq;

namespace DashboardETL.Infrastructure
{
    public static class JsonExtensions
    {
        public static JToken Select(this JObject obj, string property)
        {
            if(obj == null)
            {
                return null;
            }

            if (obj.TryGetValue(property, out JToken value))
            {
                return value;
            }

            return null;
        }

        public static JToken Select(this JToken token, string property)
        {
            if (token == null)
            {
                return null;
            }

            try
            {
                return token[property];
            }

            catch { }

            return null;
        }

        public static T Value<T>(this JToken token) 
        {
            if (token == null)
            {
                return default;
            }

            try
            {
                return token.ToObject<T>();
            }

            catch { }

            return default;
        }
    }
}
