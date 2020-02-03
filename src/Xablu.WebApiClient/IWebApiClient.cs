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

        Task<TModel> SendQueryAsync<TModel>(Request<TModel> request, CancellationToken cancellationToken = default(CancellationToken))
        where TModel : class, new();

        Task<TModel> SendQueryAsync<TModel>(Request<TModel> request, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout, CancellationToken cancellationToken = default(CancellationToken))
            where TModel : class, new();

        Task<TModel> SendQueryAsync<TModel>(Request<TModel> request, RequestOptions options, CancellationToken cancellationToken = default(CancellationToken))
            where TModel : class, new();

        Task<TModel> SendMutationAsync<TModel>(MutationRequest<TModel> request, CancellationToken cancellationToken = default(CancellationToken))
            where TModel : class, new();

        Task<TModel> SendMutationAsync<TModel>(MutationRequest<TModel> request, RequestOptions options, CancellationToken cancellationToken = default(CancellationToken))
            where TModel : class, new();

        Task<TModel> SendMutationAsync<TModel>(MutationRequest<TModel> request, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout, CancellationToken cancellationToken = default(CancellationToken))
            where TModel : class, new();

        Task<TModel> SendMutationAsync<TModel>(string mutationString, object queryVariable, CancellationToken cancellationToken = default(CancellationToken))
            where TModel : class, new();

        Task<TModel> SendMutationAsync<TModel>(string mutationString, object queryVariable, RequestOptions options, CancellationToken cancellationToken = default(CancellationToken))
            where TModel : class, new();

        Task<TModel> SendMutationAsync<TModel>(string mutationString, object queryVariable, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout, CancellationToken cancellationToken = default(CancellationToken))
            where TModel : class, new();
    }
}
