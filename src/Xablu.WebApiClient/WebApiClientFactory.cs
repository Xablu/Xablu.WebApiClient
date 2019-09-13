using System;
using System.Net.Http;
using Xablu.WebApiClient.Client;

namespace Xablu.WebApiClient
{
    public static class WebApiClientFactory
    {
        public static IWebApiClient<T> Get<T>(string baseUrl, bool autoRedirectRequests = true, Func<DelegatingHandler> delegatingHandler = null)
        {
            var refitService = new RefitService<T>(baseUrl, autoRedirectRequests, delegatingHandler);

            return new WebApiClient<T>(refitService);
        }
    }
}
