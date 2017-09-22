using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Xablu.WebApiClient.UnitTests.Mocks
{
    internal class MockHttpResponseResolver : IHttpResponseResolver
    {
        public int CallsToResolveHttpResponse { get; private set; }

        public Task<TResult> ResolveHttpResponseAsync<TResult>(HttpResponseMessage responseMessage)
        {
            CallsToResolveHttpResponse++;

            return Task.FromResult(default(TResult));
        }
    }
}
