using System.Text.Json;
using System.Text.Json.Serialization;

namespace Imato.SimpleETL
{
    public static class Json
    {
        static Json()
        {
            JSON_OPTIONS.Converters.Add(new JsonStringEnumConverter());
        }

        public static JsonSerializerOptions JSON_OPTIONS = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public static T? Deserialize<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, JSON_OPTIONS);
            }
            catch { }
            return default;
        }

        public static string Serialize<T>(T? value)
        {
            if (value == null) return "";
            return JsonSerializer.Serialize(value, JSON_OPTIONS);
        }
    }
}