using System;
using System.Net.Http;
using Xablu.WebApiClient.Abstractions;

namespace Xablu.WebApiClient
{
    public class RestApiClientImplementation : RestApiClient
    {
        public RestApiClientImplementation(string apiBaseAddress)
            : base(apiBaseAddress)
        { }

        public RestApiClientImplementation(RestApiClientOptions restApiClientOptions)
            : base(restApiClientOptions)
        { }

        protected override Func<HttpMessageHandler> HttpMessageHandlerBuilder => () => new NSUrlSessionHandler();
    }
}
