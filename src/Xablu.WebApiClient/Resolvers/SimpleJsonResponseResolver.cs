using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xablu.WebApiClient.Resolvers
{
    public class SimpleJsonResponseResolver
        : IHttpResponseResolver
    {
        public async Task<TResult> ResolveHttpResponseAsync<TResult>(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                return default(TResult);
            }

            var responseAsString = await responseMessage.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResult>(responseAsString);
        }
    }
}
