using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Xablu.WebApiClient.Abstractions;

namespace Xablu.WebApiClient.Abstractions.Resolvers
{
    public class SimpleJsonContentResolver
        : IHttpContentResolver
    {
        private JsonSerializer _serializer;

        public SimpleJsonContentResolver(JsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public virtual HttpContent ResolveHttpContent<TContent>(TContent content)
        {
            var serializedContent = JsonConvert.SerializeObject(content);

            return new StringContent(serializedContent, Encoding.UTF8, "application/json");
        }
    }
}