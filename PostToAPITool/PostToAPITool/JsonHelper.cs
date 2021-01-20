namespace Post_to_API_Tool
{
    using Newtonsoft.Json;

    public static class JsonHelper
    {
        public static string FormatIfJson(string txt)
        {
            string result = txt;
            try
            {
                object obj = JsonConvert.DeserializeObject(txt);

                string json = JsonConvert.SerializeObject(obj, Formatting.Indented);

                result = json;
            }
            catch { }

            return result;
        }
    }
}
