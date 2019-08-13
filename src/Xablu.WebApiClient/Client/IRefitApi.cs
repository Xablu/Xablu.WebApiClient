using System;
using System.Threading.Tasks;

namespace Xablu.WebApiClient.Client
{
    public interface IRefitApi<T>
    {
        Task<T> GetAsync(string path);
        Task<T> PutAsync(string path);
        Task<T> PatchAsync(string path);
        Task<T> PostAsync(string path);
        Task<T> UpdateAsync(string path);
        Task<T> DeleteTask(string path);
    }
}