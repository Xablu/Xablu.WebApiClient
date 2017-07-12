using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Xablu.WebApiClient.UnitTests.Fakes
{
    internal static class FakeMessageHandlerExtensions
    {
        public static HttpResponseMessage When(this FakeMessageHandler handler, Uri uri)
        {
            var responseMessage = new HttpResponseMessage();
            handler.AddFakeResponse(uri, responseMessage);

            return responseMessage;
        }
    }
}
