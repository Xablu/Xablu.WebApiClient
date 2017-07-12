using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Xablu.WebApiClient.Resolvers
{
    public class SimpleStringResponseResolver
        : IHttpResponseResolver
    {
        public async Task<TResult> ResolveHttpResponseAsync<TResult>(HttpResponseMessage responseMessage)
        {
            var result = string.Empty;
            if (responseMessage.Content != null)
            {
                result = await responseMessage.Content.ReadAsStringAsync();
            }

            return (TResult) Convert.ChangeType(result, typeof(TResult));
        }
    }
}