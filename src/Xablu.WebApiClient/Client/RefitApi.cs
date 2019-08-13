using System;
using System.Threading.Tasks;

namespace Xablu.WebApiClient.Client
{
    public class RefitApi<T> : IRefitApi<T>
    {
        public async Task<T> GetAsync(string path)
        {
            var result = await IRefit.GetTask(path);
            return result;
        }

        public Task<T> PatchAsync(string path)
        {
            var result = await IRefit.PatchTask(path);
            return result;
        }

        public async Task<T> PostAsync(string path)
        {
            var result = await IRefit.PostTask(path);
            return result;
        }

        public async Task<T> PutAsync(string path)
        {
            var result = await IRefit.PutTask(path);
            return result;
        }

        public async Task<T> UpdateAsync(string path)
        {
            var result = await IRefit.UpdateTask(path);
            return result;
        }

        public async Task<T> DeleteTask(string path)
        {
            var result = await IRefit.DeleteTask(path);
            return result;
        }
    }
}