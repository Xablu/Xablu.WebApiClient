using System;
using System.Threading.Tasks;
using Xablu.WebApiClient.Enums;

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
    }
}
