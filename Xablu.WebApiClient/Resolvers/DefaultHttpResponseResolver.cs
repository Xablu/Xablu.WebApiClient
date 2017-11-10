using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Xablu.WebApiClient.Resolvers
{
    internal class DefaultHttpResponseResolver
        : IHttpResponseResolver
    {
        private JsonSerializer _serializer;

        public DefaultHttpResponseResolver(JsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public async Task<TResult> ResolveHttpResponseAsync<TResult>(HttpResponseMessage responseMessage)
        {
            //If you need logging for development, use #if DEBUG and JsonConvert otherwise
#if DEBUG
            using (var stream = await responseMessage.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            {
                string text = await reader.ReadToEndAsync();
                var jobject = JObject.Parse(text);
                var result = (string) jobject.SelectToken("Result");

                Debug.WriteLine("RECEIVED: " + result);
                return JsonConvert.DeserializeObject<TResult>(result);
            }
#else
            var responseAsString = await responseMessage.Content.ReadAsStringAsync();

            var result = JObject.Parse(responseAsString);
            return JsonConvert.DeserializeObject<TResult>((string)result.SelectToken("Result"));

            //TODO: Find a way to use stream here
            /*
            using (var stream = await responseMessage.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var json = new JsonTextReader(reader))
            {
                return _serializer.Deserialize<TResult>(json);
            }
            */
#endif
        }
    }
}