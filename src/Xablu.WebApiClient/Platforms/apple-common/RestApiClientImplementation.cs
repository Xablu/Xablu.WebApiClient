//using System;
//using System.Net.Http;
//using Foundation;

//namespace Xablu.WebApiClient
//{
//    public class RestApiClientImplementation : RestApiClient
//    {
//        public RestApiClientImplementation(string apiBaseAddress)
//            : base(new RestApiClientOptions { ApiBaseAddress = apiBaseAddress })
//        {
//        }

//        public RestApiClientImplementation(RestApiClientOptions options)
//            : base(options)
//        {
//        }

//        protected override Func<HttpMessageHandler> HttpMessageHandlerBuilder => () => new NSUrlSessionHandler();
//    }
//}
