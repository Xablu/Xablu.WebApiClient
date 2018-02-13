using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Xablu.WebApiClient.Resolvers;
using Newtonsoft.Json;
using System.IO;

namespace Sample.Resolvers
{
    public class TestResolver : SimpleJsonResponseResolver
    {
        public TestResolver(JsonSerializer serializer) : base(serializer)
        {
        }

        public virtual async Task<TResult> ResolveHttpResponseAsync<TResult>(HttpResponseMessage responseMessage)
        {
            using (var stream = await responseMessage.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<TResult>(text);
            }
        }
    }
}
