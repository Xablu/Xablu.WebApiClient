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

        Task<TResponseModel> SendQueryAsync<TResponseModel>(QueryRequest<TResponseModel> request, CancellationToken cancellationToken = default)
            where TResponseModel : class, new();

        Task<TResponseModel> SendQueryAsync<TResponseModel>(QueryRequest<TResponseModel> request, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout, CancellationToken cancellationToken = default)
            where TResponseModel : class, new();

        Task<TResponseModel> SendQueryAsync<TResponseModel>(QueryRequest<TResponseModel> request, RequestOptions options, CancellationToken cancellationToken = default)
            where TResponseModel : class, new();

        Task<TResponseModel> SendMutationAsync<TResponseModel>(MutationRequest<TResponseModel> request, CancellationToken cancellationToken = default)
            where TResponseModel : class, new();

        Task<TResponseModel> SendMutationAsync<TResponseModel>(MutationRequest<TResponseModel> request, RequestOptions options, CancellationToken cancellationToken = default)
            where TResponseModel : class, new();

        Task<TResponseModel> SendMutationAsync<TResponseModel>(MutationRequest<TResponseModel> request, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout, CancellationToken cancellationToken = default)
            where TResponseModel : class, new();

        Task<TResponseModel> SendMutationAsync<TResponseModel>(string mutationString, object queryVariable, CancellationToken cancellationToken = default)
            where TResponseModel : class, new();

        Task<TResponseModel> SendMutationAsync<TResponseModel>(string mutationString, object queryVariable, RequestOptions options, CancellationToken cancellationToken = default)
            where TResponseModel : class, new();

        Task<TResponseModel> SendMutationAsync<TResponseModel>(string mutationString, object queryVariable, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout, CancellationToken cancellationToken = default)
            where TResponseModel : class, new();
    }
}
