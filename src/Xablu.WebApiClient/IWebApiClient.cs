using System;
using System.Threading;
using System.Threading.Tasks;
using Xablu.WebApiClient.Enums;
using Xablu.WebApiClient.Services.GraphQL;

namespace Xablu.WebApiClient
{
    public interface IWebApiClient<T>
    {
        Task Call(Func<T, Task> operation);
        Task<TResult> Call<TResult>(Func<T, Task<TResult>> operation);

        Task Call(Func<T, Task> operation, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout);
        Task<TResult> Call<TResult>(Func<T, Task<TResult>> operation, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout);

        Task Call(Func<T, Task> operation, RequestOptions options);
        Task<TResult> Call<TResult>(Func<T, Task<TResult>> operation, RequestOptions options);

        Task<TModel> SendQueryAsync<TModel>(Request<TModel> request, int retryCount, Func<Exception, bool> shouldRetry, int timeout, CancellationToken cancellationToken = default(CancellationToken))
            where TModel : class, new();

        Task<TModel> SendMutationAsync<TModel>(Request<TModel> request, int retryCount, Func<Exception, bool> shouldRetry, int timeout, CancellationToken cancellationToken = default(CancellationToken))
            where TModel : class, new();
    }
}
