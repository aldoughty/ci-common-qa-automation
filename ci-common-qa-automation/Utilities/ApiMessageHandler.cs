using System.Diagnostics;
using System.Net;
using System.Text;

namespace ci_common_qa_automation.Utilities
{
    public static class ApiMessageHandler
    {
        public static HttpResponseMessage SendMessage(HttpWebRequest requestMessage)
        {
            HttpResponseMessage response = new();
            Stopwatch timer = new();
            try
            {
                timer.Start();
                using WebResponse message = requestMessage.GetResponse();
                timer.Stop();
                using StreamReader reader = new(message.GetResponseStream());
                string text = reader.ReadToEnd();
                response.Content = new StringContent(text, Encoding.UTF8, "application/json");
            }
            catch (WebException e)
            {
                //Log Message
                HttpWebResponse? errorResponse = e.Response as HttpWebResponse;
                response.StatusCode = errorResponse?.StatusCode ?? HttpStatusCode.InternalServerError;
                return response;
            }

            return response;
        }

        public static async Task<string> SendMessage(HttpRequestMessage requestMessage)
        {
            using HttpClient client = new();
            using HttpResponseMessage response = client.SendAsync(requestMessage).Result;
            using HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            return data;
        }
        public static HttpResponseMessage SendMessageSync(HttpRequestMessage requestMessage)
        {
            using HttpClient client = new();
            return client.SendAsync(requestMessage).GetAwaiter().GetResult();
        }
    }
}
