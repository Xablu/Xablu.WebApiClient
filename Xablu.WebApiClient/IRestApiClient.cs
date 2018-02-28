using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fusillade;

namespace Xablu.WebApiClient
{
    public interface IRestApiClient : IDisposable
    {
        string AuthorizeToken { get; set; }

        Task<IRestApiResult<TResult>> GetAsync<TResult>(
            Priority priority,
            string path,
            IList<KeyValuePair<string, string>> headers = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IRestApiResult<TResult>> PatchAsync<TContent, TResult>(
            Priority priority,
            string path,
            TContent content = default(TContent),
            IList<KeyValuePair<string, string>> headers = null,
            IHttpContentResolver contentResolver = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IRestApiResult<TResult>> PostAsync<TContent, TResult>(
            Priority priority, string path,
            TContent content = default(TContent),
            IList<KeyValuePair<string, string>> headers = null,
            IHttpContentResolver contentResolver = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IRestApiResult<TResult>> PutAsync<TContent, TResult>(
            Priority priority,
            string path,
            TContent content = default(TContent),
            IList<KeyValuePair<string, string>> headers = null,
            IHttpContentResolver contentResolver = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IRestApiResult<TResult>> DeleteAsync<TResult>(
            Priority priority,
            string path,
            IList<KeyValuePair<string, string>> headers = null,
            IHttpResponseResolver httpResponseResolver = null,
            CancellationToken cancellationToken = default(CancellationToken));

        void ConfigureOptions(RestApiClientOptions options);
        void ChangeBaseAddress(string newBaseAdress);
    }
}