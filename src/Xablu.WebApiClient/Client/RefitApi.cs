using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xablu.WebApiClient.Enums;
using Xablu.WebApiClient.Services;

namespace Xablu.WebApiClient.Client
{
    public class RefitApi<T> : IRefitApi<T>
    {
        private readonly RefitService<T> _refitService;

        public RefitApi(string baseUrl = "", Func<DelegatingHandler> delegatingHandler = null)
        {
            _refitService = new RefitService<T>(baseUrl, delegatingHandler);
        }
        public async Task Call(string path, Priority priority, Func<T, Task> operation)
        {
            var service = GetServiceByPriority(priority);

            await operation.Invoke(service);
        }

        public async Task<TResult> Call<TResult>(string path, Priority priority, Func<T, Task<TResult>> operation)
        {
            var service = GetServiceByPriority(priority);

            return await operation.Invoke(service);
        }

        private T GetServiceByPriority(Priority priority)
        {
            switch (priority)
            {
                case Priority.Background:
                    return _refitService.Background;
                case Priority.Speculative:
                    return _refitService.Speculative;
                case Priority.UserInitiated:
                default:
                    return _refitService.UserInitiated;
            }
        }
    }
}