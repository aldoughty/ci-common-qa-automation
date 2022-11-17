using Newtonsoft.Json;

namespace ci_common_qa_automation.Utilities
{
    public static class JsonHelper
    {
        public static object? TryJsonConvert<T>(T jsonType, string json)
        {
            object? result = null;
            try
            {
                result = JsonConvert.DeserializeAnonymousType(json, jsonType);
            }
            catch (Exception) { return result; }
            return result;
        }
    }
}
