using System.Net.Http;
using System.Threading.Tasks;

namespace Xablu.WebApiClient.Abstractions
{
    public interface IHttpResponseResolver
    {
        Task<TResult> ResolveHttpResponseAsync<TResult>(HttpResponseMessage responseMessage);
    }
}