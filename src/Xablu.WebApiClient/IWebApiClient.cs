using System;
using System.Threading.Tasks;
using Xablu.WebApiClient.Enums;

namespace Xablu.WebApiClient
{
    public interface IWebApiClient<T>
    {
        Task Call(Priority priority, Func<T, Task> operation);
        Task<TResult> Call<TResult>(Priority priority, Func<T, Task<TResult>> operation);
    }
}
