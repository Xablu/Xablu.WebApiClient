using System.Net;

namespace Xablu.WebApiClient
{
    public interface IRestApiResult<out TResult>
    {
        bool IsSuccessStatusCode { get; }
        HttpStatusCode HttpStatusCode { get; }
        string ReasonPhrase { get; }
        string DetailedErrorMessage { get; }
        TResult Content { get; }
    }
}