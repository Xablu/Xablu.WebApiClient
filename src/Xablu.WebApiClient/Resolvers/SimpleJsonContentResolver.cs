using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Xablu.WebApiClient.Resolvers
{
    public class SimpleJsonContentResolver
        : IHttpContentResolver
    {
        private JsonSerializer _serializer;

        public SimpleJsonContentResolver(JsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public HttpContent ResolveHttpContent<TContent>(TContent content)
        {
            var serializedContent = JsonConvert.SerializeObject(content);

            return new StringContent(serializedContent, Encoding.UTF8, "application/json");
        }
    }
}
