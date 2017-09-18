using System;
using System.Threading.Tasks;
using Xablu.WebApiClient.HttpExtensions;

namespace Xablu.WebApiClient
{
    public class BaseClient
    {
        protected readonly IWebApiClient apiClient;

        public BaseClient(IWebApiClient apiClient)
        {
            if (apiClient == null)
                throw new ArgumentNullException(nameof(apiClient));

            this.apiClient = apiClient;
        }

        protected virtual Task<TResult> ExecuteRemoteRequest<TResult>(Func<Task<TResult>> action)
        {
            return WebApiClientPollyExtensions.PollyDecorator(
                action,
                3,
                (retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}