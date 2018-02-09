using System.Net;
using System.Text.RegularExpressions;

namespace Xablu.WebApiClient.Abstractions
{
    public class RestApiResult<TResult>
        : IRestApiResult<TResult>
    {
        public RestApiResult(HttpStatusCode httpStatusCode, TResult content = default(TResult),
            string reasonPhrase = null)
        {
            HttpStatusCode = httpStatusCode;
            Content = content;
            ReasonPhrase = reasonPhrase;
        }

        public RestApiResult(HttpStatusCode httpStatusCode, TResult content = default(TResult),
            string reasonPhrase = null, string detailedErrorMessage = null)
        {
            HttpStatusCode = httpStatusCode;
            Content = content;
            ReasonPhrase = reasonPhrase;
            DetailedErrorMessage = detailedErrorMessage;
        }

        public bool IsSuccessStatusCode => Regex.IsMatch(((int) HttpStatusCode).ToString(), "^2\\d\\d$");
        public HttpStatusCode HttpStatusCode { get; }
        public string ReasonPhrase { get; }
        public string DetailedErrorMessage { get; }
        public TResult Content { get; }
    }
}