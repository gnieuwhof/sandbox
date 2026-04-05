namespace TokenTool
{
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public static class Serializer
    {
        public static readonly JsonSerializerOptions OPTIONS = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };


        public static T Deserialize<T>(string json)
        {
            T result = JsonSerializer.Deserialize<T>(json, OPTIONS);

            return result;
        }

        public static string Serialize(object obj)
        {
            string result = JsonSerializer.Serialize(obj, OPTIONS);

            return result;
        }
    }
}
