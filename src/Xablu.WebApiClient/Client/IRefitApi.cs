using System;
using System.Threading.Tasks;
using Xablu.WebApiClient.Enums;

namespace Xablu.WebApiClient.Client
{
    public interface IRefitApi<T>
    {
        Task Call(string path, Priority priority, Func<T, Task> operation);
        Task<TResult> Call<TResult>(string path, Priority priority, Func<T, Task<TResult>> operation);
    }
}