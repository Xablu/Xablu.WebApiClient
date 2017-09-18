using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fusillade;

namespace Xablu.WebApiClient
{
    public interface IRestApiClient : IDisposable
    {
        void SetBaseAddress(string apiBaseAddress);

        string AcceptHeader { get; set; }
        IDictionary<string, string> Headers { get; }
        IHttpContentResolver HttpContentResolver { get; set; }
        IHttpResponseResolver HttpResponseResolver { get; set; }

        Task<IRestApiResult<TResult>> GetAsync<TResult>(Priority priority, string path,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IRestApiResult<TResult>> PatchAsync<TContent, TResult>(Priority priority, string path,
            TContent content = default(TContent), IHttpContentResolver contentResolver = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IRestApiResult<TResult>> PostAsync<TContent, TResult>(Priority priority, string path,
            TContent content = default(TContent), IHttpContentResolver contentResolver = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IRestApiResult<TResult>> PutAsync<TContent, TResult>(Priority priority, string path,
            TContent content = default(TContent), IHttpContentResolver contentResolver = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IRestApiResult<TResult>> DeleteAsync<TResult>(Priority priority, string path,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}