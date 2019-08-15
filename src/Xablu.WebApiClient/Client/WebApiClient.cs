using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xablu.WebApiClient.Client;
using Xablu.WebApiClient.Enums;
using Xablu.WebApiClient.Services;

namespace Xablu.Client.WebApiClient.Client
{
    public class WebApiClient<T> : IWebApiClient<T>
    {
        private readonly RefitService<T> _refitService;
        private readonly RefitApiClient _refitApiClient;

        public WebApiClient(string baseUrl = "", Func<DelegatingHandler> delegatingHandler = null)
        {
            _refitService = new RefitService<T>(baseUrl, delegatingHandler);
            _refitApiClient = new RefitApiClient(baseUrl, delegatingHandler);
        }

        public async Task Call(Priority priority, Func<T, Task> operation)
        {
            var service = GetServiceByPriority(priority);

            await operation.Invoke(service);
        }

        public async Task<TResult> Call<TResult>(Priority priority, Func<T, Task<TResult>> operation)
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

    public class RefitApiClient
    {
        private readonly RefitApi<IRefit> _refitApi;
        private readonly WebApiClient<IRefit> _refitClient;
        public RefitApiClient(string baseUrl = "", Func<DelegatingHandler> delegatingHandler = null)
        {
            _refitApi = new RefitApi<IRefit>();
            _refitClient = new WebApiClient<IRefit>(baseUrl, delegatingHandler);
        }
        public WebApiClient<IRefit> Client
        {
            get
            {
                return _refitClient;
            }
        }
    }
}
