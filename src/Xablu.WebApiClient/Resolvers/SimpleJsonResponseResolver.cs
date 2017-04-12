using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xablu.WebApiClient.Resolvers
{
    public class SimpleJsonResponseResolver
        : IHttpResponseResolver
    {
        private JsonSerializer _serializer;

        public SimpleJsonResponseResolver(JsonSerializer serializer)
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
                string text = reader.ReadToEnd();
                Debug.WriteLine("RECEIVED: " + text);
                return JsonConvert.DeserializeObject<TResult>(text);
            }

            #else
            using (var stream = await responseMessage.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var json = new JsonTextReader(reader))
            {
                return _serializer.Deserialize<TResult>(json);
            }
            #endif
        }
    }
}
