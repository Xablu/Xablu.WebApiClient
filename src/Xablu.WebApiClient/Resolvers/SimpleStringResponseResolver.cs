using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xablu.WebApiClient;

namespace Xablu.WebApiClient.Resolvers
{
    public class SimpleStringResponseResolver
        : IHttpResponseResolver
    {
        public virtual async Task<TResult> ResolveHttpResponseAsync<TResult>(HttpResponseMessage responseMessage)
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