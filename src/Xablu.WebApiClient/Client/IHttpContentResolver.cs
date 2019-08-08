using System.Net.Http;

namespace Xablu.WebApiClient
{
    public interface IHttpContentResolver
    {
        HttpContent ResolveHttpContent<TContent>(TContent content);
    }
}