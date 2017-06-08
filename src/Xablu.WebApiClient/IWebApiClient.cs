using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fusillade;

namespace Xablu.WebApiClient
{
    [Obsolete("Interface `IWebApiClient` is deprecated, please use the `IRestApiClient` interface instead.")]
    public interface IWebApiClient : IDisposable
    {
        void SetBaseAddress(string apiBaseAddress);

        string AcceptHeader { get; set; }
        IDictionary<string, string> Headers { get; }
        IHttpContentResolver HttpContentResolver { get; set; }
        IHttpResponseResolver HttpResponseResolver { get; set; }

        Task<TResult> GetAsync<TResult>(Priority priority, string path, CancellationToken cancellationToken = default(CancellationToken));
        Task<HttpResponseMessage> GetAsync(Priority priority, string path, CancellationToken cancellationToken = default(CancellationToken));
        Task<TResult> PostAsync<TContent, TResult>(Priority priority, string path, TContent content = default(TContent), IHttpContentResolver contentResolver = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<TResult> PutAsync<TContent, TResult>(Priority priority, string path, TContent content = default(TContent), IHttpContentResolver contentResolver = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<TResult> DeleteAsync<TResult>(Priority priority, string path, CancellationToken cancellationToken = default(CancellationToken));
    }
}