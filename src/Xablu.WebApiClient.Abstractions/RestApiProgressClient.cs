using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fusillade;
using Xablu.WebApiClient.Abstractions.HttpContentExtensions;

namespace Xablu.WebApiClient.Abstractions
{
    public class RestApiProgressClient
        : RestApiClient
    {
        public RestApiProgressClient(string apiBaseAddress)
            : base(apiBaseAddress)
        {
        }

        public RestApiProgressClient(RestApiClientOptions options)
            : base(options)
        {
        }

        public virtual async Task<IRestApiResult<TResult>> PostAsync<TContent, TResult>(
            Priority priority,
            string path,
            TContent content = default(TContent),
            ProgressDelegate progressDelegate = null,
            IList<KeyValuePair<string, string>> headers = null,
            IHttpContentResolver httpContentResolver = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpContent = ResolveHttpContent(content);
            var stream = await httpContent.ReadAsStreamAsync();
            var progressContent = new ProgressStreamContent(httpContent.Headers, stream, cancellationToken)
            {
                Progress = progressDelegate
            };

            var httpRequestMessage = new HttpRequestMessage(new HttpMethod("POST"), path)
            {
                Content = progressContent
            };

            return await SendAsync<TResult>(priority, httpRequestMessage, headers, httpResponseResolver, cancellationToken);
        }
    }
}
