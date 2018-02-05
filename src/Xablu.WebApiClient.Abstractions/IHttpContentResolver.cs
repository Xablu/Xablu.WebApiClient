using System.Net.Http;

namespace Xablu.WebApiClient.Abstractions
{
    public interface IHttpContentResolver
    {
        HttpContent ResolveHttpContent<TContent>(TContent content);
    }
}